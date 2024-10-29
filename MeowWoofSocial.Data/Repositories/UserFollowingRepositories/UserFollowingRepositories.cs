using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.UserFollowingRepositories
{
    public class UserFollowingRepositories : GenericRepositories<UserFollowing>, IUserFollowingRepositories
    {
        public UserFollowingRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}   
