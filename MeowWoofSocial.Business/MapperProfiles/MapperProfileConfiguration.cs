using AutoMapper;
using MeowWoofSocial.Business.ApplicationMiddleware;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Enums;
using Microsoft.Data.SqlClient;
using MimeKit.Text;

namespace MeowWoofSocial.Business.MapperProfiles
{
    public class MapperProfileConfiguration : Profile
    {
        public MapperProfileConfiguration()
        {

            CreateMap<User, UserLoginResModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => TextConvert.ConvertToUnicodeEscape(src.Name)))
                .ForMember(dest => dest.Avartar, opt => opt.MapFrom(src => src.Avartar));

            CreateMap<UserRegisterReqModel, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => TextConvert.ConvertToUnicodeEscape(src.Name)))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<PostCreateReqModel, Post>()
                .ForMember(dest => dest.PostAttachments, opt => opt.Ignore())
                .ForMember(dest => dest.PostHashtags, opt => opt.Ignore());

            CreateMap<Post, PostCreateResModel>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.PostAttachments))
                .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src => src.PostHashtags))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

            CreateMap<PostAttachment, PostAttachmentResModel>()
                .ForMember(dest => dest.Attachment, opt => opt.MapFrom(src => src.Attachment));

            CreateMap<PostHashtag, PostHashtagResModel>()
                .ForMember(dest => dest.Hashtag, opt => opt.MapFrom(src => src.Hashtag));

            CreateMap<User, PostAuthorResModel>()
                .ForMember(dest => dest.Avatar, opt => opt.Ignore());

            CreateMap<UserFollowing, UserFollowingResModel>();

            CreateMap<Post, PostDetailResModel>()
                .ForMember(dest => dest.author, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => TextConvert.ConvertToUnicodeEscape(src.Content)))
                .ForMember(dest => dest.Attachment, opt => opt.MapFrom(src => src.PostAttachments
                    .Select(x => new PostAttachmentResModel
                    {
                        Id = x.Id,
                        Attachment = x.Attachment
                    }).ToList()))
                .ForMember(dest => dest.Hashtag, opt => opt.MapFrom(src => src.PostHashtags
                    .Select(x => new PostHashtagResModel
                    {
                        Id = x.Id,
                        Hashtag = x.Hashtag
                    }).ToList()))
                    
                .ForMember(dest => dest.Feeling, opt => opt.MapFrom(src => src.PostReactions
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
                    }).ToList()))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.PostReactions
                    .Where(x => x.Type == PostReactionType.Comment.ToString())
                    .Select(x => new CommentPostResModel
                    {
                        Id = x.Id,
                        Content = TextConvert.ConvertToUnicodeEscape(x.Content),
                        Attachment = x.Attachment,
                        Author = new PostAuthorResModel
                    {
                        Id = x.User.Id,
                        Name = x.User.Name
                    },
                        CreatedAt = x.CreateAt,
                        UpdatedAt = x.UpdateAt
                    }).ToList()));

            CreateMap<PostReaction, ReactionAuthorModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.Name));

            CreateMap<UserFollowingReqModel, UserFollowing>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                 .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

        }
    }
}