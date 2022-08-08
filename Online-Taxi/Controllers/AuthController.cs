using Application.Auth;
using Application.Auth.Models;
//using MediatR;
using Microsoft.AspNetCore.Http;
using wallet.lib.BaseApi.ControllerConfig;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : wallet.lib.BaseApi.ControllerConfig.ControllerBase
    {
        public AuthController(MediatR.IMediator mediator) : base(mediator)
        {

        }

        [HttpPost("Login")]
        public async Task<ActionResult<FluentResults.Result<LoginDto>>> Login([FromBody] LoginRequest request)
        {
            FluentResults.Result<LoginDto> result = await Mediator.Send(request);

            if (result.IsSuccess)
            {
                return Ok(value: result);
            }
            else
            {
                return BadRequest(error: result.ToResult());
            }
        }


       
        [HttpPost("CreateAdmin")]
        
        public async Task<ActionResult<FluentResults.Result<Guid>>> CreateAdmin(RegisterRequest request)
        {

            FluentResults.Result<Guid> result = await Mediator.Send(request);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }


    }
}
