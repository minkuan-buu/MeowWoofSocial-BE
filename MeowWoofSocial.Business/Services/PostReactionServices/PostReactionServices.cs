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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Business.Services.ReactionServices
{
    public class PostReactionServices : IPostReactionServices
    {
        private readonly IPostRepositories _postRepo;
        private readonly ICloudStorage _cloudStorage;
        private readonly IMapper _mapper;
        private readonly IHashtagRepositories _hashtagRepo;
        private readonly IUserRepositories _userRepo;
        private readonly IPostAttachmentRepositories _postAttachmentRepo;
        private readonly IPostReactionRepositories _postReactionRepo;
        private readonly IUserFollowingRepositories _userFollowingRepo;

        public PostReactionServices(IPostRepositories postRepo, IMapper mapper, IHashtagRepositories hashtagRepo, IUserRepositories userRepo, IPostAttachmentRepositories postAttachmentRepositories, IPostReactionRepositories postReactionRepositories, IUserFollowingRepositories userFollowingRepositories, ICloudStorage cloudStorage)
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

        public async Task<DataResultModel<CommentCreatePostResModel>> CreateComment(CommentCreateReqModel commentReq, string token)
        {
            var result = new DataResultModel<CommentCreatePostResModel>();

            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from comment due to violate of terms!");
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

                if (commentReq.Content == null && commentReq.Attachment == null)
                {
                    throw new CustomException("Comment must have content or attachment");
                }

                var NewpostReactiontId = Guid.NewGuid();
                var postReaction = _mapper.Map<PostReaction>(commentReq);
                postReaction.Id = NewpostReactiontId;
                postReaction.PostId = commentReq.PostId;
                if (commentReq.Content != null)
                {
                    postReaction.Content = TextConvert.ConvertToUnicodeEscape(commentReq.Content);
                }
                postReaction.Type = PostReactionType.Comment.ToString();
                postReaction.CreateAt = DateTime.Now;
                postReaction.UserId = userId;
                string filePath = $"post/{commentReq.PostId}/comments/{postReaction.Id}/attachments";
                if(commentReq.Attachment != null)
                {
                    var attachments = await _cloudStorage.UploadSingleFile(commentReq.Attachment, filePath);
                    postReaction.Attachment = attachments;
                }

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

        public async Task<DataResultModel<FeelingCreatePostResModel>> CreateFeeling(FeelingCreateReqModel feelingReq, string token)
        {
            var result = new DataResultModel<FeelingCreatePostResModel>();

            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from reaction due to violate of terms!");
                }

                var post = await _postRepo.GetSingle(x => x.Id == feelingReq.PostId);
                if (post == null)
                {
                    throw new CustomException("Post not found");
                }

                if (post.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Cannot reaction on an inactive post");
                }

                var NewpostReactiontId = Guid.NewGuid();
                var postReaction = _mapper.Map<PostReaction>(feelingReq);
                postReaction.Id = NewpostReactiontId;
                postReaction.PostId = feelingReq.PostId;
                postReaction.TypeReact = feelingReq.TypeReact;
                postReaction.Type = PostReactionType.Feeling.ToString();
                postReaction.CreateAt = DateTime.Now;
                postReaction.UserId = userId;
               
                await _postReactionRepo.Insert(postReaction);
                result.Data = _mapper.Map<FeelingCreatePostResModel>(postReaction);
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

        public async Task<DataResultModel<FeelingCreatePostResModel>> UpdateFeeling(FeelingCreateReqModel feelingReq, string token)
        {
            try
            {
                Guid userId = new Guid(Authentication.DecodeToken(token, "userid"));
                var user = await _userRepo.GetSingle(x => x.Id == userId);

                if (user == null || user.Status.Equals(AccountStatusEnums.Inactive))
                {
                    throw new CustomException("You are banned from reaction due to violate of terms!");
                }

                var post = await _postRepo.GetSingle(x => x.Id == feelingReq.PostId);
                if (post == null || post.Status == GeneralStatusEnums.Inactive.ToString())
                {
                    throw new CustomException("Post not found");
                }

                return new DataResultModel<FeelingCreatePostResModel>()
                {
                    Data = new FeelingCreatePostResModel()
                };

            }
            catch (Exception ex)
            {
                throw new CustomException($"An error occurred: {ex.Message}");
            }
        }
    }
}
