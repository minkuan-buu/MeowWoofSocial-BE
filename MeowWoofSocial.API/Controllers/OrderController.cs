﻿using MeowWoofSocial.Business.Services.TransactionServices;
using MeowWoofSocial.Data.DTO.RequestModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MeowWoofSocial.Data.DTO.Custom;
using Microsoft.AspNetCore.Authorization;

namespace MeowWoofSocial.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;

        public OrderController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> CreateOrder([FromBody] List<OrderDetailCreateReqModel> request)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _transactionServices.CreateOrder(request, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _transactionServices.GetOrder(id, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _transactionServices.GetOrderList(token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/address/{addressId}")]
        [Authorize(AuthenticationSchemes = "MeowWoofAuthentication")]
        public async Task<IActionResult> ChangeOrderAddress(Guid id, Guid addressId)
        {
            try
            {
                var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var result = await _transactionServices.ChangeOrderAddress(id, addressId, token);
                return Ok(result);
            }
            catch (CustomException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
