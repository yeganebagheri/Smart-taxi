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

namespace Application.Trip.CancelTrip
{
    public class CancelTripRequest : IRequest<Result<int>>
    {
        public Guid  PassengerId{ get; set; }
        public string phoneNo { get; set; }
        public class CancelTripRequestHandler : IRequestHandler<CancelTripRequest,Result<int>>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IHubContext<TripListHub, IHubService> _hub;
            private readonly IDbConnection _dbConnection;
            private readonly IRedisServices _redisServices;
            public CancelTripRequestHandler(IRedisServices redisServices, IDbConnection dbConnection,
                Infrastructure.IUnitOfWork unitOfWork, IHubContext<TripListHub, IHubService> hub)
            {
                _unitOfWork = unitOfWork;
                _dbConnection = dbConnection;
                _redisServices = redisServices;
                _hub = hub;
            }

            public async Task<Result<int>> Handle(CancelTripRequest request, CancellationToken cancellationToken)
            {
                var result = new Result<int>();
                //get trip-req
                var DParameter = new DynamicParameters();
                DParameter.Add("@passengerId", request.PassengerId);
                var trip_req = await _dbConnection.QueryFirstAsync<Trip_req>("select *  FROM [dbo].[Trip_req] where passengerId=@passengerId and IsFinish = 0 ", DParameter);

                //delete trip-req
                var DParameter2 = new DynamicParameters();
                DParameter2.Add("@Id", trip_req.Id);
                var Deleted_Trip_req = await _dbConnection.ExecuteAsync("delete  FROM [dbo].[Trip_req] where Id=@Id ", DParameter2);

              

                return result.WithValue(Deleted_Trip_req);
            }
        }

    }
}
