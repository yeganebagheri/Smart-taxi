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
using System.Threading;
using System.Threading.Tasks;

namespace Application.Trip.Req_Trip
{
    public class TripReqRequest : IRequest<Result<int>>
    {
        public LocationModel sourceAndDest { get; set; }

        //public int firstPrice { get; set; }

        public int passesNum { get; set; }

        public Guid passengerId { get; set; }
        public bool IsFinish { get; set; }
        public class TripReqRequestHandler : IRequestHandler<TripReqRequest, Result<int>>
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
             
            public async Task<Result<int>> Handle(TripReqRequest request, CancellationToken cancellationToken)
            {
                var result = new Result<int>();

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

                //get user model from passengerId (Requesting passenger)
                var DParameter = new DynamicParameters();
                DParameter.Add("@Id", request.passengerId);
                var UserId = _dbConnection.QueryFirst<Guid>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter);

                var DParameter2 = new DynamicParameters();
                DParameter2.Add("@Id", UserId);
                var user = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter2);

                //check this user doesn`t request twice
                var DParameter0 = new DynamicParameters();
                DParameter0.Add("@passengerId", request.passengerId);
                var trip_reqExist = await _dbConnection.QueryAsync<Guid?>("SELECT Id  FROM [dbo].[Trip_req] where passengerId=@passengerId and IsFinish=0", DParameter0);
                if(trip_reqExist.Count() != 0)
                {
                    return result.WithValue(0);
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
                    Id = Guid.NewGuid(),
                    IsFinish = false
                };

                await _unitOfWork.TripReq.InsertTripReq(trip_req);

                ////get nearest trip request with source
                //var nearestOrigin = await _unitOfWork.TripReq.GetNearestOrigins(request.sourceAndDest.SLatitude, request.sourceAndDest.SLongitude);
                //var nearestSourseReq = nearestOrigin.AsList();


                //get nearest trip request with destination from which are nearest origins
                var nearestdest = await _unitOfWork.TripReq.GetNearestDest(request.sourceAndDest.SLatitude, request.sourceAndDest.SLongitude,
                    request.sourceAndDest.DLatitude, request.sourceAndDest.DLongitude , request.passesNum ,request.passengerId);

                var nearestDestReq = nearestdest.AsList();

                if (nearestDestReq.Count==0)
                {
                    return result.WithValue(3);
                }

                //match number of passeNum
                if(request.passesNum==1)
                {

                }else if(request.passesNum==2)
                {

                };
                //get nearest Driver request with source
                var nearestDriverOrigin = await _unitOfWork.TripReq.GetNearestDriverOrigins(request.sourceAndDest.SLatitude, request.sourceAndDest.SLongitude);
                var nearestDriversSourseReq = nearestDriverOrigin.AsList();
                if (nearestDriversSourseReq.Count == 0)
                {
                    return result.WithValue(3);
                }
                var nearestDriverSourseReq = nearestDriversSourseReq.GroupBy(x => x.DriverId).Select(s => s.First()).ToList();


                //create trip with nearest triprequest
                if (trip_req.passesNum == 0)
                {
                    

                    //Core.Entities.DataModels.SubPreTrip subPreTrip = new()
                    //{
                    //    SLongitude = request.sourceAndDest.SLongitude,
                    //    SLatitude = request.sourceAndDest.SLatitude,
                    //    DLongitude = request.sourceAndDest.DLongitude,
                    //    DLatitude = request.sourceAndDest.DLatitude,
                    //    username = user.username,
                    //    phoneNo = user.phoneNo,
                    //    Id = Guid.NewGuid()
                    //};
                    //await _unitOfWork.SubPreTripRepository.InsertSubPreTrip(subPreTrip);

                    Core.Entities.DataModels.PreTrip pretrip = new()
                    {
                        SLongitude1 = request.sourceAndDest.SLongitude,
                        SLatitude1 = request.sourceAndDest.SLatitude,
                        DLongitude1 = request.sourceAndDest.DLongitude,
                        DLatitude1 = request.sourceAndDest.DLatitude,
                        username1 = user.username,
                        phoneNo1 = user.phoneNo,
                        //
                        IsProcessed = false,
                        Id = Guid.NewGuid()

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip);
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {

                    };
                    await _unitOfWork.TripReq.UpdateIsFinishTripReq(request.passengerId, 1);
                    List<PreTrip> pretripList = new();
                   //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {

                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        pretripList.Add(pretrip);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriver(pretripList);
                        //await _hub.invoke(SendToList);

                    };

                    //result.

                }
                else if (trip_req.passesNum == 1)
                {
                   

                    //get user model from passengerId 2
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", nearestDestReq[0].passengerId);
                    var UserId1 = _dbConnection.QueryFirst<Guid?>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter1);

                    //Companion not found
                    if (UserId1 == null)
                    {
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(request.passengerId, 0);
                        return result.WithValue(3);
                    }
                    //Companion found
                    else if (UserId1 != null)
                    {
                        var connectionId = await _redisServices.GetFromRedis(nearestDestReq[0].passengerId);
                        await _hub.Clients.Client(connectionId).BroadcastOutfitResultToPassnger(1);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[0].passengerId , 1);
                    }


                    var DParameter4 = new DynamicParameters();
                    DParameter4.Add("@Id", UserId1);
                    var user4 = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter4);


                    // get source and dest with locationId
                    var DParameter3 = new DynamicParameters();
                    DParameter3.Add("@Id", nearestDestReq[0].LocationId);
                    var locModel = _dbConnection.QueryFirst<LocationModel>("SELECT *  FROM [dbo].[LocationModel] where Id=@Id ", DParameter3);

                    
                    

                    Core.Entities.DataModels.PreTrip pretrip2 = new()
                    {
                        SLongitude1 = request.sourceAndDest.SLongitude,
                        SLatitude1 = request.sourceAndDest.SLatitude,
                        DLongitude1 = request.sourceAndDest.DLongitude,
                        DLatitude1 = request.sourceAndDest.DLatitude,
                        username1 = user.username,
                        phoneNo1 = user.phoneNo,
                        //2
                        SLongitude2 = locModel.SLongitude,
                        SLatitude2 = locModel.SLatitude,
                        DLongitude2 = locModel.DLongitude,
                        DLatitude2 = locModel.DLatitude,
                        username2 = user4.username,
                        phoneNo2 = user4.phoneNo,
                        //
                        IsProcessed = false,
                         Id = Guid.NewGuid()

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip2);

                  
                    List<PreTrip> pretripList = new();
                    //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {
                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        pretripList.Add(pretrip2);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriver(pretripList);
                        //await _hub.invoke(SendToList);

                    };


                }
                else if (trip_req.passesNum == 2)
                {
                    
                    //get user model from passengerId 2
                    var DParameter1 = new DynamicParameters();
                    DParameter1.Add("@Id", nearestDestReq[0].passengerId);
                    var UserId1 = _dbConnection.QueryFirst<Guid?>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter1);


                    var DParameter6 = new DynamicParameters();
                    DParameter6.Add("@Id", nearestDestReq[1].passengerId);
                    var UserId2 = _dbConnection.QueryFirst<Guid?>("SELECT userId  FROM [dbo].[Passenger] where Id=@Id ", DParameter6);

                    //Companions not found
                    if (UserId1 == null && UserId2 == null)
                    {
                        return result.WithValue(3);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(request.passengerId, 0);
                    }
                    //both Companion found
                    else if (UserId1 != null && UserId2 != null)
                    {
                        //passenger1
                        var connectionId1 = await _redisServices.GetFromRedis(nearestDestReq[0].passengerId);
                        await _hub.Clients.Client(connectionId1).BroadcastOutfitResultToPassnger(1);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[0].passengerId , 1);
                        //passenger2
                        var connectionId2 = await _redisServices.GetFromRedis(nearestDestReq[1].passengerId);
                        await _hub.Clients.Client(connectionId2).BroadcastOutfitResultToPassnger(1);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[1].passengerId , 1);
                    }
                    //one Companion found another one not found
                    else if (UserId1 != null && UserId2 == null)
                    {
                        //passenger1
                        var connectionId1 = await _redisServices.GetFromRedis(nearestDestReq[0].passengerId);
                        await _hub.Clients.Client(connectionId1).BroadcastOutfitResultToPassnger(1);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[0].passengerId, 1);
                        //passenger2
                        var connectionId2 = await _redisServices.GetFromRedis(nearestDestReq[1].passengerId);
                        await _hub.Clients.Client(connectionId2).BroadcastOutfitResultToPassnger(1);
                        await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[1].passengerId, 1);
                    }

                    var DParameter4 = new DynamicParameters();
                    DParameter4.Add("@Id", UserId1);
                    var user4 = _dbConnection.QueryFirst<User>("SELECT *  FROM [dbo].[User] where Id=@Id ", DParameter4);

                    //get user model from passengerId 3
                   

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


                   

                    Core.Entities.DataModels.PreTrip pretrip1 = new()
                    {
                        SLongitude1 = request.sourceAndDest.SLongitude,
                        SLatitude1 = request.sourceAndDest.SLatitude,
                        DLongitude1 = request.sourceAndDest.DLongitude,
                        DLatitude1 = request.sourceAndDest.DLatitude,
                        username1 = user.username,
                        phoneNo1 = user.phoneNo,
                        //2
                        SLongitude2 = locModel.SLongitude,
                        SLatitude2 = locModel.SLatitude,
                        DLongitude2 = locModel.DLongitude,
                        DLatitude2 = locModel.DLatitude,
                        username2 = user4.username,
                        phoneNo2 = user4.phoneNo,
                        //3
                        SLongitude3 = locModel.SLongitude,
                        SLatitude3 = locModel.SLatitude,
                        DLongitude3 = locModel.DLongitude,
                        DLatitude3 = locModel.DLatitude,
                        username3 = user4.username,
                        phoneNo3 = user4.phoneNo,
                        //
                        Id = Guid.NewGuid(),
                        IsProcessed = false

                    };
                    await _unitOfWork.PreTripRepository.InsertPreTrip(pretrip1);

                    List<PreTrip> pretripList = new();
                    //send to hub
                    foreach (var driver in nearestDriverSourseReq)
                    {
                        var connectionId = await _redisServices.GetFromRedis(driver.DriverId);
                        //await _hub.Clients.Client(connectionId).BroadcastTripToDriver("broadcastTripList", preTrips);
                        pretripList.Add(pretrip1);
                        await _hub.Clients.Client(connectionId).BroadcastTripToDriver(pretripList);
                        //await _hub.invoke(SendToList);

                    };

                };

                result.WithSuccess("درخواست سفر ثبت شد");

                return result;


            }
        }
        
    }

}


