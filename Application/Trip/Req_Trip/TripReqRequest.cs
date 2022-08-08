
using Core.Entities;
using FluentResults;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Trip.Req_Trip
{
    public class TripReqRequest : IRequest<Result>
    {
        public LocationModel sourceAndDest { get; set; }

        public int firstPrice { get; set; }

        public int passesNum { get; set; }

        public Guid passengerId { get; set; }

        public class TripReqRequestHandler : IRequestHandler<TripReqRequest, Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;
            private readonly IMediator _mediator;

            public TripReqRequestHandler(IMediator mediator, Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
                _mediator = mediator;
            }

            public async Task<Result> Handle(TripReqRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();


                var d1 = request.sourceAndDest.SLatitude * (Math.PI / 180.0);
                var num1 = request.sourceAndDest.SLongitude * (Math.PI / 180.0);
                var d2 = request.sourceAndDest.DLatitude * (Math.PI / 180.0);
                var num2 = request.sourceAndDest.DLongitude * (Math.PI / 180.0) - num1;
                var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
                var distance =  6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
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
                    passesNum = request.passesNum
                };
                //if (trip_req.passesNum == 0)
                //{
                //    // add to list of tripRequest which Driver can see
                    
                //   await _mediator.Send(trip_req);

                //}

                await _unitOfWork.TripReq.InsertTripReq(trip_req);

                result.WithSuccess("ثبت نام انجام شد!");
                
                return result;

            }

        }
    }

}


