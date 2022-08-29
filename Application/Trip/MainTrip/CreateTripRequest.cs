using Application.Hubs;
using Application.Services;
using Core.Entities;
using Core.Entities.DataModels;
using Dapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Trip.MainTrip
{
    public class CreateTripRequest : IRequest<Result>
    {
        public Guid driverId { get; set; }
        public Guid PretripId { get; set; }
        //public static User pass3 { get; set; }

        public class CreateTripRequestHandler : IRequestHandler<CreateTripRequest, Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IHubContext<TripListHub, IHubService> _hub;
            private readonly IDbConnection _dbConnection;
            private readonly IRedisServices _redisServices;
            public CreateTripRequestHandler(IRedisServices redisServices, 
                IDbConnection dbConnection, Infrastructure.IUnitOfWork unitOfWork,
                IHubContext<TripListHub, IHubService> hub)
            {
                _unitOfWork = unitOfWork;
                _dbConnection = dbConnection;
                _redisServices = redisServices;
                _hub = hub;
            }

            public async Task<Result> Handle(CreateTripRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();

                //get priTrip
                var DParameter4 = new DynamicParameters();
                DParameter4.Add("@Id", request.PretripId);
                var preTrip = _dbConnection.QueryFirst<PreTrip>("SELECT *  FROM [dbo].[PreTrip] where Id=@Id ", DParameter4);

                //get driver
                var DParameter5 = new DynamicParameters();
                DParameter5.Add("@Id", request.driverId);
                var driver = _dbConnection.QueryFirst<Core.Entities.Driver>("SELECT *  FROM [dbo].[Driver] where Id=@Id ", DParameter5);

                var DParameter6 = new DynamicParameters();
                DParameter6.Add("@Id", driver.userId);
                var driverUser = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter6);

                //create driverRes model
                DriverRes driverRes = new()
                {
                    car= driver.car,
                    carId= driver.carId,
                    gender = driverUser.gender,
                    username = driverUser.username,
                    phoneNo= driverUser.phoneNo
                };

                //Update IsReady Driver
                await _unitOfWork.DriverRep.UpdateIsReadyDriverReq(request.driverId);

                //passenger1 
                var DParameter7 = new DynamicParameters();
                DParameter7.Add("@phoneNo", preTrip.phoneNo1);
                var user = _dbConnection.QueryFirst<Core.Entities.User>("SELECT *  FROM [dbo].[User] where phoneNo=@phoneNo ", DParameter7);

                var DParameter8 = new DynamicParameters();
                DParameter8.Add("@Id", user.Id);
                var passenger = _dbConnection.QueryFirst<Core.Entities.Passenger>("SELECT *  FROM [dbo].[Passenger] where userId=@Id", DParameter8);

                //send to hub 
                var connectionId = await _redisServices.GetFromRedis(passenger.Id);
                await _hub.Clients.Client(connectionId).BroadcastDriverResultToPassnger(driverRes);
                

                //passenger2
                if (preTrip.phoneNo2 != null)
                {
                    var DParameter9 = new DynamicParameters();
                    DParameter9.Add("@phoneNo", preTrip.phoneNo2);
                    var user1 = _dbConnection.QueryFirst<Core.Entities.User>("SELECT *  FROM [dbo].[User] where phoneNo=@phoneNo ", DParameter9);

                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", user1.Id);
                    var passenger1 = _dbConnection.QueryFirst<Core.Entities.Passenger>("SELECT *  FROM [dbo].[Passenger] where userId=@Id", DParameter1);

                    //send to hub 
                    var connectionId1 = await _redisServices.GetFromRedis(passenger1.Id);
                    await _hub.Clients.Client(connectionId1).BroadcastDriverResultToPassnger(driverRes);
                }

                //passenger3
                if (preTrip.phoneNo3 != null)
                {
                    var DParameter9 = new DynamicParameters();
                    DParameter9.Add("@phoneNo", preTrip.phoneNo3);
                    var user1 = _dbConnection.QueryFirst<Core.Entities.User>("SELECT *  FROM [dbo].[User] where phoneNo=@phoneNo ", DParameter9);

                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", user1.Id);
                    var passenger1 = _dbConnection.QueryFirst<Core.Entities.Passenger>("SELECT *  FROM [dbo].[Passenger] where userId=@Id", DParameter1);


                    //send to hub 
                    var connectionId1 = await _redisServices.GetFromRedis(passenger1.Id);
                    await _hub.Clients.Client(connectionId1).BroadcastDriverResultToPassnger(driverRes);

                }


                if (preTrip.IsProcessed)
                {
                    result.WithError("!این سفر توسط یک راننده دیگر انتخاب شده");
                }

                
                await _unitOfWork.PreTripRepository.UpdateIsProcessedPreTrip(request.PretripId);


                Core.Entities.Trip trip = new()
                {
                    DriverId = request.driverId,
                    PreTripId = request.PretripId,
                    isCancled = false,
                    price = "2000",

                };
                //await _unitOfWork.Trips.InsertTrip(trip);
                // send to hub for driver
                //await TripListHub.SendUsersConnected(trip);


                result.WithSuccess("سفر ثبت شد");

                return result;


            }
        }
    }
}
