//using Core.Entities;
//using Core.Entities.DataModels;
//using Dapper;
//using FluentResults;
//using Infrastructure;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.Auth
//{
//    public class GetProfileRequest : IRequest<Result<ProfileDto>>
//    {
//        public Guid UserId { get; set; }

//        public class GetProfileRequestHandler : IRequestHandler<GetProfileRequest, Result<ProfileDto>>
//        {
//            private readonly IUnitOfWork _unitOfWork;
//            private readonly IMediator _mediator;


//            public GetProfileRequestHandler(IUnitOfWork unitOfWork, IMediator mediator)
//            {
//                _unitOfWork = unitOfWork;
//                _mediator = mediator;
//            }


//            public async Task<Result<ProfileDto>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
//            {

//                var result = new Result<ProfileDto>();
//                //var user = await _unitOfWork.Users.GetUserByPhone(request.PhoneNo);
//                ////user exists
//                //if (user == null)
//                //{
//                //    return result.WithError("شماره موبایل وارد شده اشتباه است. لطفا دوباره تلاش کنید.");
//                //}
//                var user = await _unitOfWork.Users.GetUser(request.UserId);

//                if (user == null)
//                {
//                    return result.WithError("کاربر پیدا نشد. لطفا ثبت نام کنید.");
//                }

//                //var DParameter = new DynamicParameters();
//                //DParameter.Add("@PhoneNo", request.PhoneNo);
//                //var user1 = _unitOfWork.DbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where phoneNo=@PhoneNo ", DParameter);
//                if (user.role == 1)
//                {
//                    var DParameter1 = new DynamicParameters();
//                    DParameter1.Add("@UserId", user.Id);
//                    var passenger = _unitOfWork.DbConnection.QueryFirst<Passenger>("SELECT *  FROM [dbo].[Passenger] where userId=@UserId ", DParameter1);
//                    ProfileDto loginDto = new()
//                    {
//                        user = user,
//                        passenger = passenger
//                    };
//                    result.WithSuccess("با موفقیت وارد شدید!");
//                    result.WithValue(loginDto);
//                }
//                else if (user.role == 2)
//                {
//                    var DParameter2 = new DynamicParameters();
//                    DParameter2.Add("@UserId", user.Id);
//                    var DriverId = _unitOfWork.DbConnection.QueryFirst<Guid>("SELECT Id  FROM [dbo].[Driver] where userId=@UserId ", DParameter2);
//                    result.WithSuccess("با موفقیت وارد شدید!");
//                    LoginDto loginDto = new()
//                    {
//                        user = user1,
//                        passOrDriverId = DriverId
//                    };
//                    result.WithValue(loginDto);
//                }



//                return result;
//            }
//        }
//    }
//}
