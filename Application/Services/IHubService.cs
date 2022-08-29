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
        Task BroadcastTripToDriver(List<PreTrip> preTrips);
        Task BroadcastOutfitResultToPassnger(int Result,string price );

        Task BroadcastDriverResultToPassnger(DriverRes driverRes);
        Task ConnectionOn(string connectionID);
    }
}
