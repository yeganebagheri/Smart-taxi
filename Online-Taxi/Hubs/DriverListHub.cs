using Core.Entities;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
//using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Online_Taxi.Hubs
{
    public class DriverListHub :  Hub
    {
        private static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DriverListHub>();

        private readonly IDictionary<string, UserConnection> _connections;

        public DriverListHub(IDictionary<string, UserConnection> connections)
        {
            _connections = connections;
        }

        public async Task<string> SendRequest(string message)
        {

            return message;

        }

       
        public Task SendUsersConnected(List<Trip> trips)
        {
            return context.Clients.Client(Context.ConnectionId).SendAsync("broadcastTripList", trips);
        }


    }
}
