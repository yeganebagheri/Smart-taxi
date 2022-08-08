using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Passenger
{
    public interface IPassengerRepository
    {
        public Task InsertPassenger(Core.Entities.Passenger passenger);
    }

    public class PassengerRepository : IPassengerRepository
    {
        private readonly IDbConnection _dbConnection;
        public PassengerRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public async Task InsertPassenger(Core.Entities.Passenger passenger)
        {
            await _dbConnection.InsertAsync<Core.Entities.Passenger>(passenger);
        }
    }
}
