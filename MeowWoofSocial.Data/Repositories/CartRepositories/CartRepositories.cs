using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;

namespace MeowWoofSocial.Data.Repositories.CartRepositories
{
    public class CartRepositories : GenericRepositories<Cart>, ICartRepositories
    {
        public CartRepositories(MeowWoofSocialContext context)
       : base(context)
       {
       }
    }

}