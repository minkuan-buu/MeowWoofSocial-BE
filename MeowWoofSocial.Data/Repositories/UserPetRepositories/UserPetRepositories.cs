using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.UserPetRepositories
{
    public class UserPetRepositories : GenericRepositories<UserPet>, IUserPetRepositories
    {
        public UserPetRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}