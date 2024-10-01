using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.PostRepositories
{
    public class PostRepositories : GenericRepositories<Post>, IPostRepositories
    {
        public PostRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
    }
