using Application.Auth;
using Application.Auth.Models;
//using MediatR;
using Microsoft.AspNetCore.Http;
using wallet.lib.BaseApi.ControllerConfig;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Core.Entities.DataModels;
using FluentResults;

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


       
        [HttpPost("Register")]
        public async Task<ActionResult<FluentResults.Result<Guid>>> Register(RegisterRequest request)
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


        [HttpPost("Profile")]
        public async Task<ActionResult<ProfileDto>> GetProfile(GetProfileRequest request)
        {

            FluentResults.Result<ProfileDto> result = await Mediator.Send(request);

            if (result.IsSuccess)
            {
                return Ok(value: result);
            }
            else
            {
                return BadRequest(result);
            }
        }



    }
}
