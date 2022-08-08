using Application.Trip;
using Application.Trip.MainTrip;
using Application.Trip.Req_Trip;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Online_Taxi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : wallet.lib.BaseApi.ControllerConfig.ControllerBase
    {
        public TripController(MediatR.IMediator mediator) : base(mediator)
        {

        }

        [HttpPost("TripRequest")]
        public async Task<ActionResult<FluentResults.Result>> TripRequest([FromBody] TripReqRequest request)
        {
            FluentResults.Result result = await Mediator.Send(request);

            if (result.IsSuccess)
            {
                return Ok(value: result);
            }
            else
            {
                return BadRequest(error: result);
            }
        }

        [HttpPost("CreateTrip")]
        public async Task<ActionResult<FluentResults.Result>> CreateTrip([FromBody] CreateTripRequest request)
        {
            FluentResults.Result result = await Mediator.Send(request);

            if (result.IsSuccess)
            {
                return Ok(value: result);
            }
            else
            {
                return BadRequest(error: result);
            }
        }
    }
}
