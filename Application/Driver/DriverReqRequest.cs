using Core.Entities;
using Core.Entities.DataModels;
using Dapper;
using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Driver
{
    public class DriverReqRequest : IRequest<List<PreTrip>>
    {

        public double SLongitude { get; set; }
        public double SLatitude { get; set; }

        //public string ConnectionId { get; set; }

        public string DriverId { get; set; }


        public class DriverReqRequestHandler : IRequestHandler<DriverReqRequest, List<PreTrip>>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;
            private readonly IDbConnection _dbConnection;

            public DriverReqRequestHandler(IDbConnection dbConnection, IMediator mediator, Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
                _dbConnection = dbConnection;
            }

            public async Task<List<PreTrip>> Handle(DriverReqRequest request, CancellationToken cancellationToken)
            {
                var result = new Result<List<PreTrip>>();


                //insert in LocationModel
                //Core.Entities.LocationModel locationModel = new()
                //{
                //    SLatitude = request.sourceAndDest.SLatitude,
                //    SLongitude = request.sourceAndDest.SLongitude,
                //    DLatitude = request.sourceAndDest.DLatitude,
                //    DLongitude = request.sourceAndDest.DLongitude,
                //    Id = Guid.NewGuid()
                //};

                //await _unitOfWork.LocRep.InsertLoc(locationModel);
                try
                {

                    //insert in Driver_req
                    Core.Entities.DriverReq driver_req = new()
                {
                    SLongitude = request.SLongitude,
                    SLatitude = request.SLatitude,
                    DriverId = new Guid(request.DriverId),
                    IsReady = true,
                    Id = Guid.NewGuid()
                };

               
                    await _unitOfWork.DriverReqRep.InsertDriverReq(driver_req);
                }
                catch(Exception ex)
                {
                    result = new Result<List<PreTrip>>();

                }
                

                //get PreTrips which have IsProcessed = false
                var preTrips = _dbConnection.Query<PreTrip>("SELECT *  FROM [dbo].[PreTrip] pt where pt.IsProcessed=0 ");
                var preTripsList = preTrips.AsList();
                List<PreTrip> ListpreTrips = new List<PreTrip>();
                foreach(var preTrip in preTripsList)
                {
                    //Computing nearest origins 
                    var d1 = request.SLatitude * (Math.PI / 180.0);
                    var num1 = request.SLongitude * (Math.PI / 180.0);
                    var d2 = preTrip.SLatitude1 * (Math.PI / 180.0);
                    var num2 = preTrip.SLongitude1 * (Math.PI / 180.0) - num1;
                    var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                             Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
                    var distance = 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
                    if (distance < 27644445)
                    {
                       // await _unitOfWork.PreTripRepository.UpdateIsProcessedPreTrip(preTrip.Id);
                        ListpreTrips.Add(preTrip);
                    }
                }

                return ListpreTrips;



            }
        }
    }
}
