﻿using _3_Infrastructure.Repositories.Driver;
using _3_Infrastructure.Repositories.Passenger;
using _3_Infrastructure.Repositories.Trip;
using Infrastructure.Repositories.User;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class UnitOfWork : /*wallet.lib.dapper.UnitOfWork,*/ IUnitOfWork
    {
        public bool IsDisposed { get; set; }
        public IUserRepository Users { get; }
        public ITripReqRepository TripReq { get; }
        public IDbConnection DbConnection { get; }
        
        public ILocationRepository LocRep { get; }

        public IDriverRepository DriverRep { get; }
        public IPassengerRepository passengerRep { get; }

        public UnitOfWork(
            ILocationRepository locRep,
            IDriverRepository driverRepository,
            IUserRepository usersRepository,
            IPassengerRepository PassengerRepository,
            ITripReqRepository TripsRepository,
           IDbConnection dbConnection)


        {
            Users = usersRepository;
            TripReq = TripsRepository;
            DbConnection = dbConnection;
            LocRep = locRep;
            passengerRep = PassengerRepository;
            DriverRep = driverRepository;

        }

        // public void Dispose()
        // {
        //     Dispose(true);

        //     GC.SuppressFinalize(this);
        // }


        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {

            }


            IsDisposed = true;
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            DbConnection.Dispose();
        }
    }
}
