using MeowWoofSocial.Business.Services.TransactionServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;

        public PaymentController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> Payment([FromBody] Guid OrderId)
        {
            var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            var result = await _transactionServices.CreatePaymentUrl(token, OrderId);
            return Ok(new { redirectUrl = result });
        }
    }
}
