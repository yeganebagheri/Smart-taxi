using Core.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IHubService
    {
        Task BroadcastTripToDriver(string name, List<PreTrip> preTrips);
        Task BroadcastTripToDriverNot(string name, PreTrip preTrips);
    }
}
