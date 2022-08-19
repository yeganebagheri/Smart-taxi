﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Trip
{
    public interface ITripReqRepository
    {
        public Task InsertTripReq(Core.Entities.Trip_req user);
        public Task<IEnumerable<Trip_req>> GetNearestOrigins(double lat1 , double long2 );
        public Task<IEnumerable<Trip_req>> GetNearestDest(double lat1, double long2);
        public Task<IEnumerable<DriverReq>> GetNearestDriverOrigins(double lat1, double long2);
    }
}
