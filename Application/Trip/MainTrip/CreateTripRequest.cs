using Application.Hubs;
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
            private readonly IHubContext<Hub> _hub;
            private readonly IDbConnection _dbConnection;
            public CreateTripRequestHandler(IDbConnection dbConnection, Infrastructure.IUnitOfWork unitOfWork/*, IHubContext<TripListHub> hub*/)
            {
                _unitOfWork = unitOfWork;
                _dbConnection = dbConnection;
                // _hub = hub;
            }

            public async Task<Result> Handle(CreateTripRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();

                //get priTrip
                var DParameter4 = new DynamicParameters();
                DParameter4.Add("@Id", request.PretripId);
                var preTrip = _dbConnection.QueryFirst<PreTrip>("SELECT *  FROM [dbo].[PreTrip] where Id=@Id ", DParameter4);

                if(preTrip.IsProcessed)
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
