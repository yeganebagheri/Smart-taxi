using FluentResults;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using wallet.lib.dapper;
using Dapper.FastCrud;

namespace Application.Auth
{
    public class RegisterRequest : IRequest<Result>
    {
        public string Username { get; set; }

        public string PhoneNo { get; set; }

        public string Password { get; set; }

        public bool gender { get; set; }
        public bool role { get; set; }

        public class RegisterRequestHandler : IRequestHandler<RegisterRequest, Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            
            public RegisterRequestHandler(Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(RegisterRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();
                Core.Entities.User user = new()
                {
                    Id = Guid.NewGuid(),
                    phoneNo = request.PhoneNo,
                    username = request.Username,
                    Password = request.Password,
                    gender = request.gender,
                    role = request.role
                };
                await _unitOfWork.Users.InsertUser(user);

                result.WithSuccess("ثبت نام انجام شد!");
                
                return result;

            }

        }
    }

}


