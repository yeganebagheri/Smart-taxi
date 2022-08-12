using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Pre_Trip
{
    public interface IPreTripRepository
    {
        public Task InsertPreTrip(Core.Entities.DataModels.PreTrip subPreTrip);
        public class PreTripRepository : IPreTripRepository
        {
            private readonly IDbConnection _dbConnection;
            public PreTripRepository(IDbConnection dbConnection)
            {
                _dbConnection = dbConnection;
            }
            public async Task InsertPreTrip(Core.Entities.DataModels.PreTrip subPreTrip)
            {

                await _dbConnection.InsertAsync<Core.Entities.DataModels.PreTrip>(subPreTrip);


            }
        }
    }
}
