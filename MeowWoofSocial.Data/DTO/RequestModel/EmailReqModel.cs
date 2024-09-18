using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class EmailReqModel
    {
        public string Email { get; set; } = null!;
        public string HtmlContent { get; set; } = null!;
    }
}
