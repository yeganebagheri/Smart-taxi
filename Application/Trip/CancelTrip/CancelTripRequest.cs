using Application.Hubs;
using Application.Services;
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
    public class CancelTripRequest : IRequest<Result>
    {
        public Guid PreTripId { get; set; }
        public Guid  PhoneNum{ get; set; }
        public class CancelTripRequestHandler : IRequestHandler<CancelTripRequest,Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IHubContext<TripListHub, IHubService> _hub;
            private readonly IDbConnection _dbConnection;
            public CancelTripRequestHandler(IDbConnection dbConnection, Infrastructure.IUnitOfWork unitOfWork, IHubContext<TripListHub, IHubService> hub)
            {
                _unitOfWork = unitOfWork;
                _dbConnection = dbConnection;
                _hub = hub;
            }

            public async Task<Result> Handle(CancelTripRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();
                //var DParameter3 = new DynamicParameters();
                //DParameter3.Add("@Id", request.PreTripId);
                //var preTrip = _dbConnection.QueryFirst<PreTrip>("SELECT *  FROM [dbo].[PreTrip] where Id=@Id ", DParameter3);



                ////send cancel message to other
                //var connectionId = await _redisServices.GetFromRedis(preTrip.);
                //await _hub.Clients.Client(connectionId).BroadcastOutfitResultToPassnger(1);
                //await _unitOfWork.TripReq.UpdateIsFinishTripReq(nearestDestReq[0].passengerId, 1);
                ////delete PriTrip
                //await _unitOfWork.PreTripRepository.DeletePreTrip(request.PreTripId);

                return result;
            }
        }

    }
}
