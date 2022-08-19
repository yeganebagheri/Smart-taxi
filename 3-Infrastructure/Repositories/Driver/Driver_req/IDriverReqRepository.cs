using Core.Entities;
using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Driver.Driver_req
{
    public interface IDriverReqRepository
    {
        public Task InsertDriverReq(Core.Entities.DriverReq driverReq);
    }

    public class DriverReqRepository : IDriverReqRepository
    {
        private readonly IDbConnection _dbConnection;
        public DriverReqRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task InsertDriverReq(Core.Entities.DriverReq driverReq)
        {

             await _dbConnection.InsertAsync<DriverReq>(driverReq);


        }
    }
}
