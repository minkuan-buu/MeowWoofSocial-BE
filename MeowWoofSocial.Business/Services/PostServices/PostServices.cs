using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.Custom;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using MeowWoofSocial.Data.Repositories.HashtagRepositories;
using MeowWoofSocial.Data.Repositories.PostAttachmentRepositories;
using MeowWoofSocial.Data.Repositories.PostRepositories;
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

        public PostServices(IPostRepositories postRepo, IMapper mapper, IHashtagRepositories hashtagRepo, IUserRepositories userRepo, IPostAttachmentRepositories postAttachmentRepositories)
        {
            _postRepo = postRepo;
            _mapper = mapper;
            _hashtagRepo = hashtagRepo;
            _userRepo = userRepo;
            _postAttachmentRepo = postAttachmentRepositories;
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
    }
}

