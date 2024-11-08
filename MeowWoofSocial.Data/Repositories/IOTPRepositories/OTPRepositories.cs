using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.IOTPRepositories
{
    public class OTPRepositories : GenericRepositories<Otp>, IOTPRepositories
    {
        public OTPRepositories(MeowWoofSocialContext context) : base(context)
        {
        }
    }
}
