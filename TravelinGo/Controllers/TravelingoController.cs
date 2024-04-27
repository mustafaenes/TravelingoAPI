using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelinGo.Business;
using TravelinGo.Business.Models;
using TravelinGo.Business.Requests;

namespace TravelinGo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowReactApp")]
    public class TravelingoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IGeneralManager _GeneralManager;

        public TravelingoController(IConfiguration configuration, IGeneralManager generalManager)
        {
            _configuration = configuration;
            _GeneralManager = generalManager;
        }

        [HttpGet("GetCities")]
        public IActionResult GetCities()
        {
            var result = _GeneralManager.GetCities();
            return Ok(result);
        }

        [HttpPost("SignUpUser")]
        public TravelingoResponse SignUpUser(GeneralRequests request)
        {
            var result = _GeneralManager.SignUpUser(request);
            return result;
        }

        [HttpGet("GetRestaurantDetails/{LocationId}")]
        public string GetRestaurantDetails(int LocationId)
        {
            var result = _GeneralManager.GetRestaurantDetails(LocationId);
            return result;
        }

        [HttpGet("GetCommentsByLocationId/{locationId}")]
        public string GetComments(int locationId)
        {
            var result = _GeneralManager.GetCommentsByLocationId(locationId);
            return result;
        }

        [HttpPost("AddOrUpdateComment")]
        public TravelingoResponse AddOrUpdateComment(Comments comment)
        {
            var result = _GeneralManager.AddOrUpdateComment(comment);
            return result;
        }

        [HttpPost("GetChatGPTResponse")]
        public async Task<IActionResult> GetChatGPTResponse(GptRequest request)
        {
            try
            {
                var response = await _GeneralManager.GetChatGPTResponse(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

    }
}
