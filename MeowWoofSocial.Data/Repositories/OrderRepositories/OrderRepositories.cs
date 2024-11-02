﻿using MeowWoofSocial.Data.Entities;
using MeowWoofSocial.Data.Repositories.GenericRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.Repositories.OrderRepositories
{
    public class OrderRepositories : GenericRepositories<Order>, IOrderRepositories
    {
        public OrderRepositories(MeowWoofSocialContext context)
        : base(context)
        {
        }
    }
}