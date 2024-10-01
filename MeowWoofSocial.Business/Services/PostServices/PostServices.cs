using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.PostServices
{
    public class PostServices : IPostServices
    {
        private readonly IPostRepositories _postRepo;
        private readonly IMapper _mapper;
        private readonly IHashtagRepositories _hashtagRepo;
        private readonly IUserRepositories _userRepo;
        private readonly IPostAttachmentRepositories _postAttachmentRepo;
        private readonly IPostReactionRepositories _postReactionRepo;
        private readonly IUserFollowingRepositories _userFollowingRepo;

        public PostServices(IPostRepositories postRepo, IMapper mapper, IHashtagRepositories hashtagRepo, IUserRepositories userRepo, IPostAttachmentRepositories postAttachmentRepositories, IPostReactionRepositories postReactionRepositories, IUserFollowingRepositories userFollowingRepositories)
        {
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

                var postEntity = _mapper.Map<Post>(post);
                postEntity.UserId = user.Id;
                postEntity.CreateAt = DateTime.Now;
                postEntity.Status = GeneralStatusEnums.Active.ToString();

                await _postRepo.Insert(postEntity);

                if (post.Attachment != null)
                {
                    var attachments = post.Attachment.Select(att => new PostAttachment
                    {
                        Id = Guid.NewGuid(),
                        PostId = postEntity.Id,
                        Attachment = att,
                        Status = GeneralStatusEnums.Active.ToString()
                    }).ToList();
                    postEntity.PostAttachments = attachments;
                    await _postAttachmentRepo.InsertRange(attachments);
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
        public async Task<ListDataResultModel<PostDetailResModel>> GetNewsFeed(string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));

                var followingEntities = await _userFollowingRepo.GetList(
                    x => x.UserId.Equals(userId),
                    includeProperties: "Follower.Posts.PostReactions.User"
                );

                var followedPosts = new List<PostDetailResModel>();

                foreach (var following in followingEntities)
                {
                    foreach (var followerPost in following.Follower.Posts)
                    {
                        var postDetail = new PostDetailResModel()
                        {
                            Id = followerPost.Id,
                            author = _mapper.Map<PostAuthorResModel>(following.Follower),
                            Attachments = _mapper.Map<List<PostAttachmentResModel>>(followerPost.PostAttachments),
                            Content = followerPost.Content,
                            CreateAt = followerPost.CreateAt,
                            Status = followerPost.Status,
                            updatedAt = followerPost.UpdateAt,
                            Feeling = followerPost.PostReactions
                                .Where(x => x.Type.Equals(PostReactionType.Feeling.ToString()))
                                .Select(x => new FeelingPostResModel
                                {
                                    Id = x.Id,
                                    TypeReact = x.TypeReact,
                                    Author = _mapper.Map<PostAuthorResModel>(x.User)
                                })
                                .ToList(),

                            Comment = followerPost.PostReactions
                                .Where(x => x.Type.Equals(PostReactionType.Comment.ToString()))
                                .Select(x => new CommentPostResModel
                                {
                                    Id = x.Id,
                                    Content = x.Content,
                                    Attachment = x.Attachment,
                                    Author = _mapper.Map<PostAuthorResModel>(x.User),
                                    CreatedAt = x.CreateAt,
                                    UpdatedAt = x.UpdateAt
                                })
                                .ToList()
                        };

                        followedPosts.Add(postDetail);
                    }
                }

                var nonFollowedPosts = await _postRepo.GetList(
                    x => !followingEntities.Select(f => f.FollowerId).Contains(x.UserId),
                    includeProperties: "User,PostReactions.User"
                );

                var otherPosts = new List<PostDetailResModel>();
                foreach (var post in nonFollowedPosts)
                {
                    var postDetail = new PostDetailResModel()
                    {
                        Id = post.Id,
                        author = _mapper.Map<PostAuthorResModel>(post.User),
                        Attachments = _mapper.Map<List<PostAttachmentResModel>>(post.PostAttachments),
                        Content = post.Content,
                        CreateAt = post.CreateAt,
                        Status = post.Status,
                        updatedAt = post.UpdateAt,
                        Feeling = post.PostReactions
                            .Where(x => x.Type.Equals(PostReactionType.Feeling.ToString()))
                            .Select(x => new FeelingPostResModel
                            {
                                Id = x.Id,
                                TypeReact = x.TypeReact,
                                Author = _mapper.Map<PostAuthorResModel>(x.User)
                            })
                            .ToList(),

                        Comment = post.PostReactions
                            .Where(x => x.Type.Equals(PostReactionType.Comment.ToString()))
                            .Select(x => new CommentPostResModel
                            {
                                Id = x.Id,
                                Content = x.Content,
                                Attachment = x.Attachment,
                                Author = _mapper.Map<PostAuthorResModel>(x.User),
                                CreatedAt = x.CreateAt,
                                UpdatedAt = x.UpdateAt
                            })
                            .ToList()
                    };

                    otherPosts.Add(postDetail);
                }

                var newsFeed = followedPosts.OrderByDescending(p => p.CreateAt)
                    .Concat(otherPosts.OrderByDescending(p => p.CreateAt))
                    .ToList();

                return new ListDataResultModel<PostDetailResModel>
                {
                    Data = newsFeed
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error fetching news feed: {ex.Message}");
            }
        }
    }
}

