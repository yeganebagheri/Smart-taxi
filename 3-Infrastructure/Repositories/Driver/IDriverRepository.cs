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
    }

}
