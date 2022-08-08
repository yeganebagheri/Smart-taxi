using System.Transactions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Dapper.FastCrud;

namespace Infrastructure.Repositories.User
{

    public class UserRepository :  IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        public UserRepository(IDbConnection dbConnection,IHttpContextAccessor context) 
        {
            _dbConnection = dbConnection;
        }

        public async Task<bool> PhoneExists(string phoneNo)
        {
            DynamicParameters parameters = new();
            parameters.Add("PhoneNo", phoneNo);

            string query = $"WHERE PhoneNo=@PhoneNo";
            var result = await _dbConnection.QueryAsync<Core.Entities.User>(query, parameters);
            return result.Any();
        }

        public async Task<Core.Entities.User> GetUser(Guid userId)
        {
            DynamicParameters parameters = new();
            parameters.Add("Id", userId);

            Core.Entities.User user = new()
            {
                Id = userId
            };

            string query = $"WHERE Id=@Id";
            var result = await _dbConnection.GetAsync<Core.Entities.User>(user);

            return result;
        }



        

        public async Task<Core.Entities.User> GetUserByPhone(string phoneNo)
        {
            DynamicParameters parameters = new();
            parameters.Add("@phoneNo", phoneNo);

            string query = $"SELECT * FROM dbo.[User] WHERE phoneNo = @phoneNo";

            var result = await _dbConnection.QueryAsync<Core.Entities.User>(query , parameters);

            return result.FirstOrDefault();
        }

        public async Task InsertUser(Core.Entities.User user)
        {
            
             await _dbConnection.InsertAsync<Core.Entities.User>(user);

            
        }



    }
}
