using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.HashtagRepositories;
using MeowWoofSocial.Data.Repositories.PostAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.PostReactionRepositories;
using MeowWoofSocial.Data.Repositories.PostRepositories;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.PostServices
{
    public class PostServices : IPostServices
    {
        private readonly IPostRepositories _postRepo;
        private readonly ICloudStorage _cloudStorage;
        private readonly IMapper _mapper;
        private readonly IHashtagRepositories _hashtagRepo;
        private readonly IUserRepositories _userRepo;
        private readonly IPostAttachmentRepositories _postAttachmentRepo;
        private readonly IPostReactionRepositories _postReactionRepo;
        private readonly IUserFollowingRepositories _userFollowingRepo;

        public PostServices(IPostRepositories postRepo, IMapper mapper, IHashtagRepositories hashtagRepo, IUserRepositories userRepo, IPostAttachmentRepositories postAttachmentRepositories, IPostReactionRepositories postReactionRepositories, IUserFollowingRepositories userFollowingRepositories, ICloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
            _postRepo = postRepo;
            _mapper = mapper;
            _hashtagRepo = hashtagRepo;
            _userRepo = userRepo;
            _postAttachmentRepo = postAttachmentRepositories;
            _postReactionRepo = postReactionRepositories;
            _userFollowingRepo = userFollowingRepositories;
        }

        public async Task<DataResultModel<PostCreateResModel>> CreatePost(PostCreateReqModel post, string token)
        {
            var result = new DataResultModel<PostCreateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from posting due to violate of terms!");
                }
                var NewPostId = Guid.NewGuid();
                var postEntity = _mapper.Map<Post>(post);
                postEntity.Id = NewPostId;
                postEntity.UserId = user.Id;
                postEntity.CreateAt = DateTime.Now;
                postEntity.Status = GeneralStatusEnums.Active.ToString();

                await _postRepo.Insert(postEntity);

                if (post.Attachment != null)
                {
                    string filePath = $"post/${NewPostId}/attachments";
                    List<string> GetStringURL = await _cloudStorage.UploadFile(post.Attachment, filePath);
                    List<PostAttachment> ListInsertAttachment = new();
                    foreach (var link in GetStringURL)
                    {
                        PostAttachment newAttachment = new()
                        {
                            Id = Guid.NewGuid(),
                            PostId = postEntity.Id,
                            Attachment = link,
                            Status = GeneralStatusEnums.Active.ToString()
                        };
                        ListInsertAttachment.Add(newAttachment);
                    }
                    await _postAttachmentRepo.InsertRange(ListInsertAttachment);
                }

                if (post.HashTag != null)
                {
                    var hashtags = post.HashTag.Select(ht => new PostHashtag
                    {
                        Id = Guid.NewGuid(),
                        PostId = postEntity.Id,
                        Hashtag = ht,
                        Status = GeneralStatusEnums.Active.ToString()
                    }).ToList();
                    postEntity.PostHashtags = hashtags;
                    await _hashtagRepo.InsertRange(hashtags);
                }

                var newPost = await _postRepo.GetSingle(x => x.Id == postEntity.Id, includeProperties: "PostAttachments,PostHashtags,User");
                var postResModel = _mapper.Map<PostCreateResModel>(newPost);
                result.Data = postResModel;
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error: {ex.Message}");
            }

            return result;
        }

        public async Task<ListDataResultModel<PostDetailResModel>> GetNewsFeed(string token, NewsFeedReq newsFeedReq)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));

                // Tối ưu hóa lastPostCreateAt
                DateTime? lastPostCreateAt = null;
                if (newsFeedReq.lastPostId.HasValue)
                {
                    lastPostCreateAt = (await _postRepo.GetSingle(x => x.Id == newsFeedReq.lastPostId.Value))?.CreateAt;
                }

                // Truy vấn người theo dõi và bài viết của họ
                var followingEntities = await _userFollowingRepo.GetList(
                    x => x.UserId.Equals(userId),
                    includeProperties: "Follower.Posts.PostReactions.User,Follower.Posts.PostAttachments,Follower.Posts.PostHashtags"
                );

                // Tối ưu hóa việc lấy bài viết
                var followedPostsPaging = followingEntities
                    .SelectMany(f => f.Follower.Posts)
                    .Where(p => !lastPostCreateAt.HasValue || p.CreateAt < lastPostCreateAt.Value)
                    .OrderByDescending(p => p.CreateAt)
                    .Take(newsFeedReq.PageSize)
                    .Select(MapPostDetail)  // Ánh xạ trực tiếp bằng LINQ
                    .ToList();

                // Đếm số lượng bài viết còn thiếu
                int remainingPostsCount = newsFeedReq.PageSize - followedPostsPaging.Count;

                // Lấy bài viết không theo dõi
                var nonFollowedPosts = new List<PostDetailResModel>();
                if (remainingPostsCount > 0)
                {
                    var nonFollowedPostsEntities = await _postRepo.GetList(
                        x => !followingEntities.Select(f => f.FollowerId).Contains(x.UserId)
                             && (!lastPostCreateAt.HasValue || x.CreateAt < lastPostCreateAt.Value),
                        includeProperties: "User,PostReactions.User,PostAttachments,PostHashtags"
                    );

                    nonFollowedPosts = nonFollowedPostsEntities
                        .OrderByDescending(p => p.CreateAt)
                        .Take(remainingPostsCount)
                        .Select(MapPostDetail)  // Ánh xạ trực tiếp bằng LINQ
                        .ToList();
                }

                var combinedPosts = followedPostsPaging.Concat(nonFollowedPosts).ToList();

                return new ListDataResultModel<PostDetailResModel>
                {
                    Data = combinedPosts
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error fetching news feed: {ex.Message}");
            }
        }

        // Tối ưu hóa MapPostDetail với Select trực tiếp
        private PostDetailResModel MapPostDetail(Post post)
        {
            return new PostDetailResModel
            {
                Id = post.Id,
                author = new PostAuthorResModel
                {
                    Id = post.User.Id,
                    Name = post.User.Name,
                    Avatar = post.User.Avartar
                },
                Content = TextConvert.ConvertToUnicodeEscape(post.Content),
                Attachments = post.PostAttachments.Select(x => new PostAttachmentResModel
                {
                    Id = x.Id,
                    Attachment = x.Attachment
                }).ToList(),
                Hashtags = post.PostHashtags.Select(x => new PostHashtagResModel
                {
                    Id = x.Id,
                    Hashtag = x.Hashtag
                }).ToList(),
                Feeling = post.PostReactions
                    .Where(x => x.Type == PostReactionType.Feeling.ToString())
                    .Select(x => new FeelingPostResModel
                    {
                        Id = x.Id,
                        TypeReact = x.TypeReact,
                        Author = new PostAuthorResModel
                        {
                            Id = x.User.Id,
                            Name = x.User.Name
                        }
                    }).ToList(),
                Comment = post.PostReactions
                    .Where(x => x.Type == PostReactionType.Comment.ToString())
                    .Select(x => new CommentPostResModel
                    {
                        Id = x.Id,
                        Content = TextConvert.ConvertToUnicodeEscape(x.Content),
                        Attachment = new PostAttachmentResModel()
                        {
                            Id = x.Id,
                            Attachment = x.Attachment,
                        },
                        Author = new PostAuthorResModel
                        {
                            Id = x.User.Id,
                            Name = x.User.Name
                        },
                        CreateAt = x.CreateAt,
                        UpdatedAt = x.UpdateAt
                    }).ToList(),
                CreateAt = post.CreateAt,
                UpdatedAt = post.UpdateAt
            };
        }


        public async Task<DataResultModel<PostUpdateResModel>> UpdatePost(PostUpdateReqModel postUpdateReq, string token)
        {
            var result = new DataResultModel<PostUpdateResModel>();
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var post = await _postRepo.GetSingle(x => x.Id == postUpdateReq.Id && x.UserId == userId);

                if (post == null)
                {
                    throw new CustomException("Post not found or you do not have permission to update this post");
                }

                if (post.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Cannot update an inactive post");
                }

                post.Content = postUpdateReq.Content;
                post.UpdateAt = DateTime.Now;

                if (postUpdateReq.Attachments != null && postUpdateReq.Attachments.Count > 0)
                {
                    var attachments = await _cloudStorage.UploadFile(postUpdateReq.Attachments, $"post/{post.Id}/attachments");
                    foreach (var attachment in attachments)
                    {
                        var postAttachment = new PostAttachment
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            Attachment = attachment,
                            Status = GeneralStatusEnums.Active.ToString()
                        };
                        await _postAttachmentRepo.Insert(postAttachment);
                    }
                }

                if (postUpdateReq.HashTag != null)
                {
                    var hashtags = postUpdateReq.HashTag.Select(ht => new PostHashtag
                    {
                        Id = Guid.NewGuid(), 
                        PostId = post.Id,
                        Hashtag = ht,
                        Status = GeneralStatusEnums.Active.ToString()
                    }).ToList();
                    post.PostHashtags = hashtags;
                    await _hashtagRepo.InsertRange(hashtags);
                }
                await _postRepo.Update(post);

                var updatedPost = await _postRepo.GetSingle(x => x.Id == postUpdateReq.Id, includeProperties: "PostAttachments,PostHashtags,User");

                result.Data = _mapper.Map<PostUpdateResModel>(updatedPost);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }

        public async Task<DataResultModel<CommentCreatePostResModel>> CreateComment(CommentCreateReqModel commentReq, string token)
        {
            var result = new DataResultModel<CommentCreatePostResModel>();

            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from posting due to violate of terms!");
                }

                var post = await _postRepo.GetSingle(x => x.Id == commentReq.PostId);
                if (post == null)
                {
                    throw new CustomException("Post not found");
                }

                if (post.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Cannot comment on an inactive post");
                }
                
                var NewpostReactiontId = Guid.NewGuid();
                var postReaction = _mapper.Map<PostReaction>(commentReq);
                postReaction.Id = NewpostReactiontId;
                postReaction.PostId = commentReq.PostId;
                postReaction.Content = TextConvert.ConvertToUnicodeEscape(commentReq.Content);
                postReaction.Type = PostReactionType.Comment.ToString();
                postReaction.CreateAt = DateTime.Now;
                postReaction.UserId = userId;
                string filePath = $"post/{commentReq.PostId}/comments/{postReaction.Id}/attachments";
                var attachments = await _cloudStorage.UploadSingleFile(commentReq.Attachment, filePath);
                postReaction.Attachment = attachments;

                await _postReactionRepo.Insert(postReaction);
                result.Data = _mapper.Map<CommentCreatePostResModel>(postReaction);
                result.Data.Author = new PostAuthorResModel()
                {
                    Id = user.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(user.Name),
                    Avatar = user.Avartar
                };

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }
    }
}

