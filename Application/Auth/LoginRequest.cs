using Application.Auth.Models;
using Core.Entities;
using Dapper;
using FluentResults;
using Infrastructure;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth
{
    public class LoginRequest : IRequest<Result<LoginDto>>
    {
        // [Phone]
        // [StringLength(11)]
        public string PhoneNo { get; set; }
        public string Password { get; set; }


        public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<LoginDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;


            public LoginRequestHandler(IUnitOfWork unitOfWork, IMediator mediator)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
            }


            public async Task<Result<LoginDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
            {

                var result = new Result<LoginDto>();
                var user = await _unitOfWork.Users.GetUserByPhone(request.PhoneNo);
                //user exists
                if (user == null)
                {
                    return result.WithError("شماره موبایل وارد شده اشتباه است. لطفا دوباره تلاش کنید.");
                }
                var auth = await _unitOfWork.Users.GetUser(user.Id);

                if (auth == null)
                {
                    return result.WithError("کاربر پیدا نشد. لطفا ثبت نام کنید.");
                }

                var DParameter = new DynamicParameters();
                DParameter.Add("@PhoneNo", request.PhoneNo);
                var user1 = _unitOfWork.DbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where phoneNo=@PhoneNo ", DParameter);
                if (user1.role == 1)
                {
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@UserId", user1.Id);
                    var PassengerId = _unitOfWork.DbConnection.QueryFirst<Guid>("SELECT Id  FROM [dbo].[Passenger] where userId=@UserId ", DParameter1);
                    LoginDto loginDto = new()
                    {
                        user = user1,
                        passOrDriverId = PassengerId
                    };
                    result.WithSuccess("با موفقیت وارد شدید!");
                    result.WithValue(loginDto);
                }
                else if (user1.role ==2)
                {
                    var DParameter2 = new DynamicParameters();
                    DParameter2.Add("@UserId", user1.Id);
                    var DriverId = _unitOfWork.DbConnection.QueryFirst<Guid>("SELECT Id  FROM [dbo].[Driver] where userId=@UserId ", DParameter2);
                    result.WithSuccess("با موفقیت وارد شدید!");
                    LoginDto loginDto = new()
                    {
                        user = user1,
                        passOrDriverId = DriverId
                    };
                    result.WithValue(loginDto);
                }

               

                return result;


            }
        }
    
    }
}
