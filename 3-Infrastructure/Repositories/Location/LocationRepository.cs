using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Trip
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IDbConnection _dbConnection;
        public LocationRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public  async Task InsertLoc(Core.Entities.LocationModel loc)
        {

            await _dbConnection.InsertAsync<Core.Entities.LocationModel>(loc);


        }
    }
}
