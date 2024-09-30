using AutoMapper;
using MeowWoofSocial.Data.DTO.RequestModel;
using MeowWoofSocial.Data.DTO.ResponseModel;
using MeowWoofSocial.Data.Entities;

namespace MeowWoofSocial.Business.MapperProfiles
{
    public class MapperProfileConfiguration : Profile
    {
        public MapperProfileConfiguration()
        {
            CreateMap<User, UserLoginResModel>();
            
            CreateMap<UserRegisterReqModel, User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<PostCreateReqModel, Post>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PostAttachments, opt => opt.Ignore())
                .ForMember(dest => dest.PostHashtags, opt => opt.Ignore());

            CreateMap<Post, PostCreateResModel>()
                .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.PostAttachments))
                .ForMember(dest => dest.Hashtags, opt => opt.MapFrom(src => src.PostHashtags))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User));

            CreateMap<PostAttachment, PostAttachmentResModel>();
            
            CreateMap<PostHashtag, PostHashtagResModel>();

            CreateMap<User, PostAuthorResModel>()
                .ForMember(dest => dest.Avatar, opt => opt.Ignore());

        }
    }
}
