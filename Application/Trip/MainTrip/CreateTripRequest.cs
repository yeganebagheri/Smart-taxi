using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Trip.MainTrip
{
    public class CreateTripRequest : IRequest<Result>
    {
        public Guid DriverId { get; set; }

        public bool isCancled { get; set; }

        public string price { get; set; }

        public class CreateTripRequestHandler : IRequestHandler<CreateTripRequest, Result>
        {
            private readonly Infrastructure.IUnitOfWork _unitOfWork;

            public CreateTripRequestHandler(Infrastructure.IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Result> Handle(CreateTripRequest request, CancellationToken cancellationToken)
            {
                var result = new Result();

                

                Core.Entities.Trip trip_req = new()
                {
                    DriverId = request.DriverId,
                    isCancled = request.isCancled,
                    price = request.price,
                  
                };
                //await _unitOfWork.Trips.InsertTrip(trip);

                result.WithSuccess("ثبت نام انجام شد!");

                return result;


            }



        }
    }
}
