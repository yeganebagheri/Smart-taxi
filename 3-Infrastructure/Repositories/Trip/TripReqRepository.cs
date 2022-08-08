using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Trip
{
    public class TripReqRepository : ITripReqRepository
    {
        private readonly IDbConnection _dbConnection;
        public TripReqRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public  async Task InsertTripReq(Core.Entities.Trip_req trip_req)
        {

            await _dbConnection.InsertAsync<Core.Entities.Trip_req>(trip_req);


        }
    }
}
