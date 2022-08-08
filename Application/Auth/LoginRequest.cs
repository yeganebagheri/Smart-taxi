using Application.Auth.Models;
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

                //var loginResponse = await _mediator.Send(new LoginRequest()
                //{
                //    User = user
                //});
                //var result = new Result<LoginDto>();
                //var userClaims = await _unitOfWork.UserClaims.GetByUserId(request.User.Id);

                //var deviceId = Guid.NewGuid();
                //Tokens tokens = TokenHelper.GenerateTokens(request.User, deviceId, userClaims);
                //await _unitOfWork.Devices.CreateNewDevice(deviceId, tokens, request.User.Id);

                //var roles = await _unitOfWork.Roles.GetUserRoles(request.User.Id);

                result.WithSuccess("با موفقیت وارد شدید!");
                result.WithValue(new LoginDto() { /*Tokens = tokens, */User = user/*, Roles = roles*/ });

                return result;


            }
        }
    
    }
}
