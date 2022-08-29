using Dapper;
using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Driver
{
    public interface IDriverRepository
    {
        public Task InsertDriver(Core.Entities.Driver driver);
        public Task UpdateIsReadyDriverReq(Guid driverId);
    }

    public class DriverRepository : IDriverRepository
    {
        private readonly IDbConnection _dbConnection;
        public DriverRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task InsertDriver(Core.Entities.Driver driver)
        {

            await _dbConnection.InsertAsync<Core.Entities.Driver>(driver);


        }

        public async Task UpdateIsReadyDriverReq(Guid driverId)
        {
            var DParameter = new DynamicParameters();
            DParameter.Add("@Id", driverId);
            await _dbConnection.QueryAsync("UPDATE [dbo].[DriverReq] SET IsReady = 1 where DriverId=@Id ", DParameter);
        }



    }

}
