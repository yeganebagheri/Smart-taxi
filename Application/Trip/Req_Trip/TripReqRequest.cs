using Application.Hubs;
using Application.Services;
using Core.Entities;
using Dapper;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Trip.Req_Trip
{
    public class TripReqRequest : IRequest<Result>
    {
        public LocationModel sourceAndDest { get; set; }

        //public int firstPrice { get; set; }

        public int passesNum { get; set; }

        public Guid passengerId { get; set; }

        public class TripReqRequestHandler : IRequestHandler<TripReqRequest, Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;
            private readonly IDbConnection _dbConnection;
            private readonly IRedisServices _redisServices;
            private readonly IHubContext<TripListHub , IHubService> _hub;

            public TripReqRequestHandler(IHubContext<TripListHub,IHubService> hub ,
                IRedisServices redisServices, IDbConnection dbConnection, IMediator mediator, Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
                _dbConnection = dbConnection;
                _redisServices = redisServices;
                _hub = hub;
            }
             
            public async Task<Result> Handle(TripReqRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();

                //Computing firstPrice 
                var d1 = request.sourceAndDest.SLatitude * (Math.PI / 180.0);
                var num1 = request.sourceAndDest.SLongitude * (Math.PI / 180.0);
                var d2 = request.sourceAndDest.DLatitude * (Math.PI / 180.0);
                var num2 = request.sourceAndDest.DLongitude * (Math.PI / 180.0) - num1;
                var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
                var distance = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
                var firstPrice = "";
                if (distance < 10)
                {
                    firstPrice = "10000";
                }

                //insert in LocationModel
                Core.Entities.LocationModel locationModel = new()
                {
                    SLatitude = request.sourceAndDest.SLatitude,
                    SLongitude = request.sourceAndDest.SLongitude,
                    DLatitude = request.sourceAndDest.DLatitude,
                    DLongitude = request.sourceAndDest.DLongitude,
                    Id = Guid.NewGuid()
                };

                await _unitOfWork.LocRep.InsertLoc(locationModel);


                //insert in Trip_req
                Core.Entities.Trip_req trip_req = new()
                {
                    LocationId = locationModel.Id,
                    passengerId = request.passengerId,
                    firstPrice = firstPrice,
                    passesNum = request.passesNum,
                    Id = Guid.NewGuid()
                };

                await _unitOfWork.TripReq.InsertTripReq(trip_req);

                //get nearest trip request with source
                var nearestOrigin = await _unitOfWork.TripReq.GetNearestOrigins(request.sourceAndDest.SLatitude, request.sourceAndDest.SLongitude);
                var nearestSourseReq = nearestOrigin.AsList();


                //get nearest trip request with destination from which are nearest origins
                var nearestdest = await _unitOfWork.TripReq.GetNearestDest(request.sourceAndDest.DLatitude, request.sourceAndDest.DLongitude);
                var nearestDestReq = nearestdest.AsList();

                //get nearest Driver request with source
                var nearestDriverOrigin = await _unitOfWork.TripReq.GetNearestDriverOrigins(request.sourceAndDest.SLatitude, request.sourceAndDest.SLongitude);
                var nearestDriverSourseReq = nearestDriverOrigin.AsList();
               
                //create trip with nearest triprequest
                if (trip_req.passesNum == 0)
                {
                    //get user model from passengerId (Requesting passenger)
                    var DParameter = new DynamicParameters();
                    DParameter.Add("@Id", request.passengerId);
                    var UserId = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter);

                    var DParameter2 = new DynamicParameters();
                    DParameter2.Add("@Id", UserId);
                    var user = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter2);

                    Core.Entities.DataModels.SubPreTrip subPreTrip = new()
                    {
                        SLongitude = request.sourceAndDest.SLongitude,
                        SLatitude = request.sourceAndDest.SLatitude,
                        DLongitude = request.sourceAndDest.DLongitude,
                        DLatitude = request.sourceAndDest.DLatitude,
                        username = user.username,
                        phoneNo = user.phoneNo,
                        Id = Guid.NewGuid()
                    };
                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip);

                    Core.Entities.DataModels.PreTrip pretrip = new()
                    {
                        SubPreTrip1Id = subPreTrip.Id,
                        IsProcessed = false,
                        Id = Guid.NewGuid()

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip);

                    //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {
                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriverNot("broadcastTripList", pretrip);
                        //await _hub.invoke(SendToList);

                    };

                }
                else if (trip_req.passesNum == 1)
                {
                    //get user model from passengerId (Requesting passenger)
                    var DParameter = new DynamicParameters();
                    DParameter.Add("@Id", request.passengerId);
                    var UserId = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter);

                    var DParameter2 = new DynamicParameters();
                    DParameter2.Add("@Id", UserId);
                    var user = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter2);

                    //get user model from passengerId 2
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", nearestDestReq[0].passengerId);
                    var UserId1 = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter1);

                    var DParameter4 = new DynamicParameters();
                    DParameter4.Add("@Id", UserId1);
                    var user4 = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter4);


                    // get source and dest with locationId
                    var DParameter3 = new DynamicParameters();
                    DParameter3.Add("@Id", nearestDestReq[0].LocationId);
                    var locModel = _dbConnection.QueryFirst<LocationModel>("SELECT *  FROM [dbo].[LocationModel] where Id=@Id ", DParameter3);


                    Core.Entities.DataModels.SubPreTrip subPreTrip1 = new()
                    {

                        SLongitude = request.sourceAndDest.SLongitude,
                        SLatitude = request.sourceAndDest.SLatitude,
                        DLongitude = request.sourceAndDest.DLongitude,
                        DLatitude = request.sourceAndDest.DLatitude,
                        username = user.username,
                        phoneNo = user.phoneNo,
                        Id = Guid.NewGuid()

                    };

                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip1);


                    Core.Entities.DataModels.SubPreTrip subPreTrip2 = new()
                    {
                        SLongitude =locModel.SLongitude,
                            SLatitude =locModel.SLatitude,
                            DLongitude =locModel.DLongitude,
                            DLatitude =locModel.DLatitude,
                            username =user4.username,
                            phoneNo =user4.phoneNo,
                            Id = Guid.NewGuid()

                    };
                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip2);

                    Core.Entities.DataModels.PreTrip pretrip2 = new()
                    {
                        SubPreTrip1Id = subPreTrip1.Id,
                        SubPreTrip2Id = subPreTrip2.Id,
                        IsProcessed = false

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip2);

                    //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {
                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriverNot("broadcastTripList", pretrip2);
                        //await _hub.invoke(SendToList);

                    };


                }
                else if (trip_req.passesNum == 2)
                {
                    //get user model from passengerId (Requesting passenger)
                    var DParameter = new DynamicParameters();
                    DParameter.Add("@Id", request.passengerId);
                    var UserId = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter);

                    var DParameter2 = new DynamicParameters();
                    DParameter2.Add("@Id", UserId);
                    var user = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter2);

                    //get user model from passengerId 2
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", nearestDestReq[0].passengerId);
                    var UserId1 = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter1);

                    var DParameter4 = new DynamicParameters();
                    DParameter4.Add("@Id", UserId1);
                    var user4 = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter4);

                    //get user model from passengerId 3
                    var DParameter6 = new DynamicParameters();
                    DParameter6.Add("@Id", nearestDestReq[1].passengerId);
                    var UserId2 = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter6);

                    var DParameter7 = new DynamicParameters();
                    DParameter7.Add("@Id", UserId2);
                    var user6 = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter7);


                    // get source and dest with locationId
                    var DParameter3 = new DynamicParameters();
                    DParameter3.Add("@Id", nearestDestReq[0].LocationId);
                    var locModel = _dbConnection.QueryFirst<LocationModel>("SELECT *  FROM [dbo].[LocationModel] where Id=@Id ", DParameter3);

                    // get source and dest with locationId
                    var DParameter5 = new DynamicParameters();
                    DParameter5.Add("@Id", nearestDestReq[1].LocationId);
                    var locModel1 = _dbConnection.QueryFirst<LocationModel>("SELECT *  FROM [dbo].[LocationModel] where Id=@Id ", DParameter5);


                    Core.Entities.DataModels.SubPreTrip subPreTrip1 = new()
                    {

                        SLongitude = request.sourceAndDest.SLongitude,
                        SLatitude = request.sourceAndDest.SLatitude,
                        DLongitude = request.sourceAndDest.DLongitude,
                        DLatitude = request.sourceAndDest.DLatitude,
                        username = user.username,
                        phoneNo = user.phoneNo

                    };
                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip1);

                    Core.Entities.DataModels.SubPreTrip subPreTrip2 = new()
                    {
                        SLongitude = locModel.SLongitude,
                        SLatitude = locModel.SLatitude,
                        DLongitude = locModel.DLongitude,
                        DLatitude = locModel.DLatitude,
                        username = user4.username,
                        phoneNo = user4.phoneNo

                    };
                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip2);

                    Core.Entities.DataModels.SubPreTrip subPreTrip3 = new()
                    {
                        SLongitude = locModel1.SLongitude,
                        SLatitude = locModel1.SLatitude,
                        DLongitude = locModel1.DLongitude,
                        DLatitude = locModel1.DLatitude,
                        username = user6.username,
                        phoneNo = user6.phoneNo

                    };
                    await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip3);

                    Core.Entities.DataModels.PreTrip pretrip1 = new()
                    {
                        SubPreTrip1Id = subPreTrip1.Id,
                        SubPreTrip2Id = subPreTrip2.Id,
                        SubPreTrip3Id = subPreTrip3.Id,
                        IsProcessed = false

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip1);

                    //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {
                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriverNot("broadcastTripList", pretrip1);
                        //await _hub.invoke(SendToList);

                    };

                };

                result.WithSuccess("درخواست سفر ثبت شد");

                return result;


            }
        }
        
    }

}


