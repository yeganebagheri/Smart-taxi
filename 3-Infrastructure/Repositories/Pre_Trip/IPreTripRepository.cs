using Dapper;
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
        public Task UpdateIsProcessedPreTrip(Guid passengerId);
        public Task DeletePreTrip(Guid PreTripId);
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

            public async Task UpdateIsProcessedPreTrip(Guid PreTripId)
            {
                var DParameter = new DynamicParameters();
                DParameter.Add("@Id", PreTripId);
                await _dbConnection.QueryAsync("UPDATE [dbo].[PreTrip] SET IsProcessed = 1 where Id=@Id ", DParameter);
            }


            public async Task DeletePreTrip(Guid PreTripId)
            {
                var DParameter = new DynamicParameters();
                DParameter.Add("@Id", PreTripId);
                await _dbConnection.QueryAsync("Delete [dbo].[PreTrip] where Id=@Id ", DParameter);
            }


        }
    }
}
