using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class TransactionResModel
    {
        public Guid Id { get; set; }
        public int RefId { get; set; }
        public decimal TotalPrice { get; set; }
        public string StatusPayment { get; set; } = null!;
    }
}
