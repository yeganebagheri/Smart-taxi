using _3_Infrastructure.Repositories.Driver;
using _3_Infrastructure.Repositories.Driver.Driver_req;
using _3_Infrastructure.Repositories.Passenger;
using _3_Infrastructure.Repositories.Pre_Trip;
using _3_Infrastructure.Repositories.Trip;
using Infrastructure.Repositories.User;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        public IUserRepository Users { get; }
        public ITripReqRepository TripReq { get; }
        public ILocationRepository LocRep { get; }
        public IPassengerRepository passengerRep { get; }
        public IDriverRepository DriverRep { get; }
        public IDriverReqRepository DriverReqRep { get; }
        public IDbConnection DbConnection { get; }
        public ISubPreTripRepository SubPreTripRepository { get; }
        public IPreTripRepository PreTripRepository { get; }

        public Task SaveAsync();
    }
}
