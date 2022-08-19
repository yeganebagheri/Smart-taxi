using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Driver.Driver_req
{
    public interface ISubPreTripRepository
    {
        public Task InsertSubPreTrip(Core.Entities.DataModels.SubPreTrip subPreTrip);
    }
    public class SubPreTripRepository : ISubPreTripRepository
    {
        private readonly IDbConnection _dbConnection;
        public SubPreTripRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task InsertSubPreTrip(Core.Entities.DataModels.SubPreTrip subPreTrip)
        {

            await _dbConnection.InsertAsync<Core.Entities.DataModels.SubPreTrip>(subPreTrip);


        }
    }
}
