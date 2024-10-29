using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PostAttachmentRepositories
{
    public class PostAttachmentRepositories : GenericRepositories<PostAttachment>, IPostAttachmentRepositories
    {
        public PostAttachmentRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}
