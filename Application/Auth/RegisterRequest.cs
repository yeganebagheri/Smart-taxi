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
    public class RegisterRequest : IRequest<Result<Guid>>
    {
        public string Username { get; set; }

        public string PhoneNo { get; set; }

        public string Password { get; set; }

        public int gender { get; set; }
        public int role { get; set; }
        public string car { get; set; }
        public string carId { get; set; }

        public class RegisterRequestHandler : IRequestHandler<RegisterRequest, Result<Guid>>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            
            public RegisterRequestHandler(Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Result<Guid>> Handle(RegisterRequest request, CancellationToken cancellationToken)
            {
                var result = new Result<Guid>();
                // insert in user table
                Core.Entities.User user = new()
                {
                    Id = Guid.NewGuid(),
                    phoneNo = request.PhoneNo,
                    username = request.Username,
                    Password = request.Password,
                    gender = request.gender,
                    role = request.role,
                };
                await _unitOfWork.Users.InsertUser(user);

                if(user.role == 1)
                {
                    // insert in passenger table
                    Core.Entities.Passenger passenger = new()
                    {
                        Id = Guid.NewGuid(),
                        userId = user.Id
                    };
                    await _unitOfWork.passengerRep.InsertPassenger(passenger);

                    result.WithValue(passenger.Id);

                }
                else if (user.role ==2)
                {
                    // insert in Driver table
                    Core.Entities.Driver driver = new()
                    {
                        Id = Guid.NewGuid(),
                        userId = user.Id,
                        car = request.car,
                        carId = request.carId
                    };
                    await _unitOfWork.DriverRep.InsertDriver(driver);
                    result.WithValue(driver.Id);
                }
                
                result.WithSuccess("ثبت نام انجام شد!");
                
                return result;

            }

        }
    }

}


