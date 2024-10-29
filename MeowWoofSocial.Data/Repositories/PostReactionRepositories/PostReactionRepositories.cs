using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.PostReactionRepositories
{
    public class PostReactionRepositories : GenericRepositories<PostReaction>, IPostReactionRepositories
    {
        public PostReactionRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}


