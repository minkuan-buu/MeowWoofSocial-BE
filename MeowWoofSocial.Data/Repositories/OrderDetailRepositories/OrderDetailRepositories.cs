using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.OrderRepositories
{
    public class OrderDetailRepositories : GenericRepositories<OrderDetail>, IOrderDetailRepositories
    {
        public OrderDetailRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}
