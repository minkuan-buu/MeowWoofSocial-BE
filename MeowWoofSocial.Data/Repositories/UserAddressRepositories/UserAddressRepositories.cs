using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.UserAddressRepositories
{
    public class UserAddressRepositories : GenericRepositories<UserAddress>, IUserAddressRepositories
    {
        public UserAddressRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}
