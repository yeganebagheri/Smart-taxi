using Core.Entities;
using Dapper;
using Dapper.FastCrud;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Infrastructure.Repositories.Trip
{
    public class TripReqRepository : ITripReqRepository
    {
        private readonly IDbConnection _dbConnection;
        public TripReqRepository(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public  async Task InsertTripReq(Core.Entities.Trip_req trip_req)
        {
            await _dbConnection.InsertAsync<Core.Entities.Trip_req>(trip_req);
        }


        public async Task UpdateIsFinishTripReq(Guid passengerId)
        {
            var DParameter = new DynamicParameters();
            DParameter.Add("@passengerId", passengerId);
            await _dbConnection.QueryAsync("UPDATE [dbo].[Trip_req] SET IsFinish = 1 where passengerId=@passengerId ", DParameter);
        }



        public async Task<IEnumerable<Trip_req>> GetNearestOrigins(double lat1, double long2)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Lat1", lat1);
            parameters.Add("@Long1", long2);

            var NearestTripReqList = await _dbConnection.QueryAsync<Trip_req>(
             sql: "[dbo].[get_nearest_origins]",
                param: parameters,
                 commandType: CommandType.StoredProcedure);

            return NearestTripReqList;



        }



        public async Task<IEnumerable<DriverReq>> GetNearestDriverOrigins(double lat1, double long2)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Lat1", lat1);
            parameters.Add("@Long1", long2);

            var NearestTripReqList = await _dbConnection.QueryAsync<DriverReq>(
             sql: "[dbo].[get_nearest_DriverOrigins]",
                param: parameters,
                 commandType: CommandType.StoredProcedure);

            return NearestTripReqList;



        }




        public async Task<IEnumerable<Trip_req>> GetNearestDest(double Slat, double Slong,double Dlat, double Dlong)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Slat", Slat);
            parameters.Add("@Slong", Slong);
            parameters.Add("@Dlat", Dlat);
            parameters.Add("@Dlong", Dlong);

            var NearestTripReqList = await _dbConnection.QueryAsync<Trip_req>(
             sql: "[dbo].[get_nearest_destinations]",
                param: parameters,
                 commandType: CommandType.StoredProcedure);

            return NearestTripReqList;



        }




    }
}
