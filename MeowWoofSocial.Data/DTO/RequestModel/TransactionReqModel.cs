using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MeowWoofSocial.Data.DTO.RequestModel
{
    public class TransactionReqModel
    {
    }

    public class TransactionResponseDto
    {
        [JsonProperty("error")]
        public int Error { get; set; }

        [JsonProperty("data")]
        public List<TransactionDto> Data { get; set; }
    }

    public class TransactionDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("tid")]
        public string Tid { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("cusum_balance")]
        public decimal CusumBalance { get; set; }

        [JsonProperty("when")]
        public DateTime When { get; set; }

        [JsonProperty("bank_sub_acc_id")]
        public string BankSubAccId { get; set; }

        [JsonProperty("subAccId")]
        public string SubAccId { get; set; }

        [JsonProperty("bankName")]
        public string BankName { get; set; }

        [JsonProperty("bankAbbreviation")]
        public string BankAbbreviation { get; set; }

        [JsonProperty("virtualAccount")]
        public string VirtualAccount { get; set; }

        [JsonProperty("virtualAccountName")]
        public string VirtualAccountName { get; set; }

        [JsonProperty("corresponsiveName")]
        public string CorresponsivedName { get; set; }

        [JsonProperty("corresponsiveAccount")]
        public string CorresponsiveAccount { get; set; }

        [JsonProperty("corresponsiveBankId")]
        public string CorresponsiveBankId { get; set; }

        [JsonProperty("corresponsiveBankName")]
        public string CorresponsiveBankName { get; set; }
    }
}
