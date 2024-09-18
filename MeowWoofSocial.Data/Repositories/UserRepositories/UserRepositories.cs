using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.UserRepositories
{
    public class UserRepositories : GenericRepositories<User>, IUserRepositories
    {
        public UserRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}
