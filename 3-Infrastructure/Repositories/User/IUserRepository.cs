using FluentResults;
using Infrastructure.Repositories.User.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wallet.lib.Base.Domain.Models;

namespace Infrastructure.Repositories.User
{
    public interface IUserRepository 
    {
        public Task<bool> PhoneExists(string phoneNo);
        public Task<Core.Entities.User> GetUserByPhone(string phoneNo);
        public  Task<Core.Entities.User> GetUser(Guid userId);
        public  Task InsertUser(Core.Entities.User user);
    }
}
 