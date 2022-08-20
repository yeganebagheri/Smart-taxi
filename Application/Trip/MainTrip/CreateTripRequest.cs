//using Application.Hubs;
//using FluentResults;
//using MediatR;
//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.Trip.MainTrip
//{
//    public class CreateTripRequest : IRequest<Result>
//    {
//        public Core.Entities.Driver driver { get; set; }
//        public Core.Entities.Passenger passenger { get; set; }
//        public bool isCancled { get; set; }


//        //private readonly IHubContext<TripListHub> _hub;

//        public class CreateTripRequestHandler : IRequestHandler<CreateTripRequest, Result>
//        {
//            private readonly Infrastructure.IUnitOfWork _unitOfWork;
//            private readonly IHubContext<Hub> _hub;

//            public CreateTripRequestHandler(Infrastructure.IUnitOfWork unitOfWork/*, IHubContext<TripListHub> hub*/)
//            {
//                _unitOfWork = unitOfWork;
//              // _hub = hub;
//            }

//            public async Task<Result> Handle(CreateTripRequest request, CancellationToken cancellationToken)
//            {
//                var result = new Result();

                
                
//                Core.Entities.Trip trip = new()
//                {
//                    DriverId = request.driver,
//                    PassengerId = request.DriverId,
//                    isCancled = request.isCancled,
//                    price = "2000",
                  
//                };
//                //await _unitOfWork.Trips.InsertTrip(trip);
//                // send to hub for driver
//                //await TripListHub.SendUsersConnected(trip);


//                result.WithSuccess("ثبت نام انجام شد!");

//                return result;


//            }



//        }
//    }
//}
