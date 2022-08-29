using Application.Driver;
using Application.Services;
using Core.Entities.DataModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Hubs
{
    
    public class TripListHub :  Hub<IHubService>
    {
      
        private readonly IMediator _mediator;
        private readonly IRedisServices _redisServices;

        public TripListHub(IMediator mediator , IRedisServices redisServices)
        {
            _mediator = mediator;
            _redisServices = redisServices;
        }

       
        
        // get location of driver
        public async Task SendRequest(string SLatitude, string SLongitude, string DriverId)
        {
           
            DriverReqRequest driverReq1 = new()
            {
                SLongitude = Convert.ToDouble(SLongitude),
                SLatitude = Convert.ToDouble(SLatitude),
                DriverId = DriverId
            };
            var res = await _mediator.Send(driverReq1); 
            // return message;
            await Clients.Client(Context.ConnectionId).BroadcastTripToDriver(res);

        }


        public async Task SendToList()
        {

            await Clients.All.ConnectionOn(Context.ConnectionId);

        }


        public override async Task OnConnectedAsync()
        {
           
            var httpCtx = Context.GetHttpContext();
            var someHeaderValue = httpCtx.Request.Query["access_token"].ToString();
            string[] driverOrPassId = someHeaderValue.Split(',');
            var Id = driverOrPassId[0];
            await _redisServices.SetInRedis(new Guid(Id), Context.ConnectionId);
            await base.OnConnectedAsync();
        }
         
    }
}
















//private readonly IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TripListHub>();

//private readonly IDictionary<string, UserConnection> _connections;











//public Task SendUsersConnected(Core.Entities.Trip trips)
//{
//    return context.Clients.Client(Context.ConnectionId).SendAsync("broadcastTripList", trips);
//}