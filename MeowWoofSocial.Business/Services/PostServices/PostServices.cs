using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Business.Services.CloudServices;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.HashtagRepositories;
using MeowWoofSocial.Data.Repositories.NotificationRepositories;
using MeowWoofSocial.Data.Repositories.PostAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.PostReactionRepositories;
using MeowWoofSocial.Data.Repositories.PostRepositories;
using MeowWoofSocial.Data.Repositories.PostStoredRepositories;
using MeowWoofSocial.Data.Repositories.ReportRepositories;
using MeowWoofSocial.Data.Repositories.UserFollowingRepositories;
using MeowWoofSocial.Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.WebSockets;
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
        private readonly INotificationRepositories _notificationRepo;
        private readonly IPostStoredRepositories _postStoredRepo;
        private readonly IReportRepositories _reportRepo;

        public PostServices(IPostRepositories postRepo, IMapper mapper, IHashtagRepositories hashtagRepo, IUserRepositories userRepo, IPostAttachmentRepositories postAttachmentRepositories, IPostReactionRepositories postReactionRepositories, IUserFollowingRepositories userFollowingRepositories, ICloudStorage cloudStorage, INotificationRepositories notificationRepo, IPostStoredRepositories postStoredRepo, IReportRepositories reportRepo)
        {
            _cloudStorage = cloudStorage;
            _postRepo = postRepo;
            _mapper = mapper;
            _hashtagRepo = hashtagRepo;
            _userRepo = userRepo;
            _postStoredRepo = postStoredRepo;
            _notificationRepo = notificationRepo;
            _reportRepo = reportRepo;
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
                    string filePath = $"post/{NewPostId}/attachments";
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
                        Hashtag = TextConvert.ConvertToUnicodeEscape(ht),
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
                    .Where(p => p.Status.Equals(GeneralStatusEnums.Active.ToString())) 
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
                        .Where(p => p.Status.Equals(GeneralStatusEnums.Active.ToString()))
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
                Author = new PostAuthorResModel
                {
                    Id = post.User.Id,
                    Name = TextConvert.ConvertFromUnicodeEscape(post.User.Name),
                    Avatar = post.User.Avartar
                },
                Content = TextConvert.ConvertFromUnicodeEscape(post.Content),
                Attachments = post.PostAttachments.Select(x => new PostAttachmentResModel
                {
                    Id = x.Id,
                    Attachment = x.Attachment
                }).ToList(),
                Hashtags = post.PostHashtags.Select(x => new PostHashtagResModel
                {
                    Id = x.Id,
                    Hashtag = x.Hashtag != null ? TextConvert.ConvertFromUnicodeEscape(x.Hashtag) : null
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
                            Name = TextConvert.ConvertFromUnicodeEscape(x.User.Name)
                        }
                    }).ToList(),
                Comment = post.PostReactions
                    .Where(x => x.Type == PostReactionType.Comment.ToString())
                    .OrderByDescending(x => x.CreateAt)
                    .Select(x => new CommentPostResModel
                    {
                        Id = x.Id,
                        Content = TextConvert.ConvertFromUnicodeEscape(x.Content),
                        Attachment = x.Attachment,
                        Author = new PostAuthorResModel
                        {
                            Id = x.User.Id,
                            Name = TextConvert.ConvertFromUnicodeEscape(x.User.Name)
                        },
                        CreateAt = x.CreateAt,
                        UpdatedAt = x.UpdateAt
                    }).ToList(),
                Status = post.Status,
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
                var post = await _postRepo.GetSingle(x => x.Id == postUpdateReq.Id && x.UserId == userId, includeProperties: "PostAttachments,PostHashtags");

                if (post == null)
                {
                    throw new CustomException("Post not found or you do not have permission to update this post");
                }

                if (post.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Cannot update an inactive post");
                }
                if (postUpdateReq.Content != null)
                {
                    post.Content = TextConvert.ConvertToUnicodeEscape(postUpdateReq.Content);
                } else
                {
                    throw new CustomException("Content cannot null");
                }
                post.UpdateAt = DateTime.Now;
                await _postRepo.Update(post);
                if (post.PostAttachments.Count > 0)
                {
                    await _postAttachmentRepo.DeleteRange(post.PostAttachments);
                }
                if (post.PostHashtags.Count > 0) { 
                    await _hashtagRepo.DeleteRange(post.PostHashtags);
                }

                var filePath = $"post/{post.Id}/attachments";
                await _cloudStorage.DeleteFilesInPathAsync(filePath);
                if (postUpdateReq.Attachments != null && postUpdateReq.Attachments.Count > 0)
                {
                    var attachments = await _cloudStorage.UploadFile(postUpdateReq.Attachments, filePath);
                    List<PostAttachment> ListAttachmentAdd = new();
                    foreach (var attachment in attachments)
                    {
                        var postAttachment = new PostAttachment
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            Attachment = attachment,
                            Status = GeneralStatusEnums.Active.ToString()
                        };
                        ListAttachmentAdd.Add(postAttachment);
                    }
                    await _postAttachmentRepo.InsertRange(ListAttachmentAdd);
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
                    await _hashtagRepo.InsertRange(hashtags);
                }

                var updatedPost = await _postRepo.GetSingle(x => x.Id == postUpdateReq.Id, includeProperties: "PostAttachments,PostHashtags,User,PostReactions.User");

                result.Data = _mapper.Map<PostUpdateResModel>(updatedPost);
            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
            return result;
        }

        public async Task<DataResultModel<PostDetailResModel>> GetPostByID(Guid postId, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));

                var postEntity = await _postRepo.GetSingle(
                    x => x.Id == postId,
                    includeProperties: "User,PostReactions.User,PostAttachments,PostHashtags"
                );

                if (postEntity == null)
                {
                    throw new CustomException("Post not found");
                }

                var postDetail = MapPostDetail(postEntity);

                return new DataResultModel<PostDetailResModel>
                {
                    Data = postDetail
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error fetching post by ID: {ex.Message}");
            }
        }
        public async Task<MessageResultModel> RemovePost(PostRemoveReqModel postRemoveReq, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var post = await _postRepo.GetSingle(p => p.Id == postRemoveReq.PostId, includeProperties: "PostAttachments,PostHashtags,Notifications,PostReactions,PostStoreds,Reports");

                if (post == null)
                {
                    throw new CustomException("Post not found.");
                }
                if (post.UserId != userId)
                {
                    throw new CustomException("Post is not belong to user.");
                }

                await _notificationRepo.DeleteRange(post.Notifications);
                await _cloudStorage.DeleteFilesInPathAsync($"post/{post.Id}");
                await _postAttachmentRepo.DeleteRange(post.PostAttachments);
                await _hashtagRepo.DeleteRange(post.PostHashtags);
                await _postReactionRepo.DeleteRange(post.PostReactions);
                await _postStoredRepo.DeleteRange(post.PostStoreds);
                await _reportRepo.DeleteRange(post.Reports);
                await _postRepo.Delete(post);

                var result = _mapper.Map<PostRemoveResModel>(post);
                return new MessageResultModel { 
                    Message = "Ok" 
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error removing post: {ex.Message}");
            }
        }

        public async Task<MessageResultModel> StorePost(Guid postId, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var post = await _postRepo.GetSingle(p => p.Id.Equals(postId), includeProperties: "PostStoreds");
                if (post == null)
                {
                    throw new CustomException("Post not found.");
                }
                if(post.PostStoreds.Where(x => x.UserId.Equals(userId)).FirstOrDefault() != null)
                {
                    throw new CustomException("You already store this post");
                }
                PostStored NewStored = new()
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = userId,
                    CreateAt = DateTime.Now,
                    Status = GeneralStatusEnums.Active.ToString(),
                };
                await _postStoredRepo.Insert(NewStored);
                return new MessageResultModel
                {
                    Message = "Ok"
                };
            }
            catch (Exception ex)
            {
                throw new CustomException($"Error removing post: {ex.Message}");
            }
        }

        public async Task<ListDataResultModel<PostDetailResModel>> GetUserPost(Guid userId, NewsFeedReq newsFeedReq)
        {
            try
            {
                List<PostDetailResModel> userPosts = new();
                DateTime? lastPostCreateAt = null;

                // Kiểm tra nếu có lastPostId thì lấy thời gian tạo của bài viết cuối cùng
                if (newsFeedReq.lastPostId.HasValue)
                {
                    var lastPost = await _postRepo.GetSingle(x => x.Id == newsFeedReq.lastPostId.Value);
                    lastPostCreateAt = lastPost?.CreateAt;
                }

                // Bước 1: Lấy tất cả các bài viết của người dùng, sắp xếp theo thời gian giảm dần
                var allUserPosts = await _postRepo.GetList(
                    x => x.UserId.Equals(userId), // Chỉ lấy bài viết của user
                    includeProperties: "User,PostReactions.User,PostAttachments,PostHashtags"
                );

                // Sắp xếp tất cả bài viết theo thời gian giảm dần (mới nhất trước)
                allUserPosts = allUserPosts.OrderByDescending(p => p.CreateAt).ToList();

                // Bước 2: Áp dụng lazy load (lọc bài viết cũ hơn bài cuối cùng đã load)
                if (lastPostCreateAt.HasValue)
                {
                    allUserPosts = allUserPosts
                        .Where(p => p.CreateAt < lastPostCreateAt.Value) // Lọc bài viết cũ hơn
                        .ToList();
                }

                // Giới hạn số lượng bài viết theo PageSize
                userPosts = allUserPosts
                    .Where(p => p.Status.Equals(GeneralStatusEnums.Active.ToString())) // Lọc bài viết Active
                    .Take(newsFeedReq.PageSize) // Giới hạn số lượng bài viết
                    .Select(MapPostDetail) // Ánh xạ sang PostDetailResModel
                    .ToList();

                // Trả về kết quả
                return new ListDataResultModel<PostDetailResModel>
                {
                    Data = userPosts
                };
            }
            catch (Exception ex)
            {
                // Ném ra CustomException với chi tiết lỗi
                throw new CustomException($"An error occurred while fetching posts: {ex.Message}");
            }
        }

    }
}


