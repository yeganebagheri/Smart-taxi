using Application.Driver;
using Core.Entities;
using MediatR;
//using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Hubs
{
    public class TripListHub :  Hub
    {
        //private readonly IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TripListHub>();

        //private readonly IDictionary<string, UserConnection> _connections;
        private readonly IMediator _mediator;

        public TripListHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        // get location of driver
        public async Task SendRequest(DriverReqRequest driverReq)
        {

            var res =_mediator.Send(driverReq);
           // return message;
            await Clients.Client(Context.ConnectionId).SendAsync("broadcastTripList", res);

        }

       
        //public Task SendUsersConnected(Core.Entities.Trip trips)
        //{
        //    return context.Clients.Client(Context.ConnectionId).SendAsync("broadcastTripList", trips);
        //}


    }
}
