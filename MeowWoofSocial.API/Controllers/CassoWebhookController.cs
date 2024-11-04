using MeowWoofSocial.Business.Services.TransactionServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/casso-webhook")]
    [ApiController]
    public class CassoWebhookController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;

        public CassoWebhookController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        private const string HEADER_SECURE_TOKEN = "785DC169D8B4C8FBFE2631398539C";
        [HttpPost("receive")]
        public async Task<IActionResult> ReceivePayment()
        {
            // Đọc nội dung của request body
            var requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            TransactionResponseDto jsonBody = JsonConvert.DeserializeObject<TransactionResponseDto>(requestBody);

            if (jsonBody == null)
            {
                return BadRequest("Request body is missing or invalid.");
            }

            if (jsonBody.Error != 0)
            {
                return BadRequest("Error occurred on the Casso side.");
            }
    
            // Đường dẫn tuyệt đối tới file tạm thời
            string tempPath = Path.Combine(Path.GetTempPath(), "requestBody.txt");
    
            // Ghi nội dung request vào file
            await System.IO.File.WriteAllTextAsync(tempPath, requestBody);

            // Kiểm tra Secure Token trong header
            var headers = Request.Headers;
            if (!headers.TryGetValue("Secure-Token", out var secureToken) || secureToken != HEADER_SECURE_TOKEN)
            {
                return Unauthorized("Missing or invalid Secure Token.");
            }

            var Result = await _transactionServices.HandleTransactions(jsonBody);

            return Ok(Result);
        }

    }
}
