using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowWoofSocial.Data.DTO.ResponseModel
{
    public class ResultModel
    {
    }
    public class MessageResultModel
    {
        public string Message { get; set; } = null!;
    }
    public class DataResultModel<T>
    {
        public T? Data { get; set; }
    }
    public class ListDataResultModel<T>
    {
        public List<T>? Data { get; set; }
    }
}
