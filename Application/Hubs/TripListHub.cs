using Application.Driver;
using Application.Services;
using Core.Entities.DataModels;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
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
        public async Task SendRequest(/*string ConnectionId, string DriverId, double SLatitude, double SLongitude */DriverReqRequest driverReq)
        {
            DriverReqRequest driverReq1 = new()
            {
                SLongitude= driverReq.SLongitude,
                SLatitude = driverReq.SLatitude,
                DriverId= driverReq.DriverId
            };
            var res =_mediator.Send(driverReq); 
            await _redisServices.SetInRedis(new Guid(driverReq.DriverId), Context.ConnectionId);
            // return message;
            await Clients.Client(Context.ConnectionId).BroadcastTripToDriver("broadcastTripList", res);

        }


        public async Task SendToList(string connectionId, PreTrip preTrips)
        {

            await Clients.Client(connectionId).BroadcastTripToDriverNot("broadcastTripList", preTrips);

        }



    }
}

 














//private readonly IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TripListHub>();

//private readonly IDictionary<string, UserConnection> _connections;











//public Task SendUsersConnected(Core.Entities.Trip trips)
//{
//    return context.Clients.Client(Context.ConnectionId).SendAsync("broadcastTripList", trips);
//}