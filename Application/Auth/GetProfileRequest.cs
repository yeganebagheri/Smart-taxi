using Core.Entities;
using Core.Entities.DataModels;
using Dapper;
using FluentResults;
using Infrastructure;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Auth
{
    public class GetProfileRequest : IRequest<Result<ProfileDto>>
    {
        public Guid UserId { get; set; }

        public class GetProfileRequestHandler : IRequestHandler<GetProfileRequest, Result<ProfileDto>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;


            public GetProfileRequestHandler(IUnitOfWork unitOfWork, IMediator mediator)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
            }


            public async Task<Result<ProfileDto>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
            {

                var result = new Result<ProfileDto>();
                
                var user = await _unitOfWork.Users.GetUser(request.UserId);

                if (user == null)
                {
                    return result.WithError("کاربر پیدا نشد. لطفا ثبت نام کنید.");
                }

                if (user.role == 1)
                {
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@UserId", user.Id);
                    var passenger = _unitOfWork.DbConnection.QueryFirst<Passenger>("SELECT *  FROM [dbo].[Passenger] where userId=@UserId ", DParameter1);
                    ProfileDto loginDto = new()
                    {
                        user = user,
                        passenger = passenger
                    };
                    result.WithSuccess("با موفقیت وارد شدید!");
                    result.WithValue(loginDto);
                }
                else if (user.role == 2)
                {
                    var DParameter2 = new DynamicParameters();
                    DParameter2.Add("@UserId", user.Id);
                    var driver = _unitOfWork.DbConnection.QueryFirst<Core.Entities.Driver>("SELECT *  FROM [dbo].[Driver] where userId=@UserId ", DParameter2);
                    result.WithSuccess("با موفقیت وارد شدید!");
                    ProfileDto loginDto = new()
                    {
                        user = user,
                        driver = driver
                    };
                    result.WithValue(loginDto);
                }



                return result;
            }
        }
    }
}
