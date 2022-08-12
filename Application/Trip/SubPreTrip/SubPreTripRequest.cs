//using FluentResults;
//using MediatR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Application.Trip.SubPreTrip
//{
//    public class SubPreTripRequest : IRequest<Result>
//    {

//        public LocationModel sourceAndDest { get; set; }

//        public string ConnectionId { get; set; }

//        public Guid DriverId { get; set; }


//        public class DriverReqRequestHandler : IRequestHandler<DriverReqRequest, Result>
//        {
//            private readonly Infrastructure.IUnitOfWork _unitOfWork;
//            private readonly IMediator _mediator;
//            private readonly IDbConnection _dbConnection;

//            public DriverReqRequestHandler(IDbConnection dbConnection, IMediator mediator, Infrastructure.IUnitOfWork unitOfWork)
//            {
//                _unitOfWork = unitOfWork;
//                _mediator = mediator;
//                _dbConnection = dbConnection;
//            }

//            public async Task<Result> Handle(DriverReqRequest request, CancellationToken cancellationToken)
//            {
//                var result = new Result();


//                //insert in LocationModel
//                Core.Entities.LocationModel locationModel = new()
//                {
//                    SLatitude = request.sourceAndDest.SLatitude,
//                    SLongitude = request.sourceAndDest.SLongitude,
//                    DLatitude = request.sourceAndDest.DLatitude,
//                    DLongitude = request.sourceAndDest.DLongitude,
//                    Id = Guid.NewGuid()
//                };

//                await _unitOfWork.LocRep.InsertLoc(locationModel);

//            }
//        }
//    }
//}       
