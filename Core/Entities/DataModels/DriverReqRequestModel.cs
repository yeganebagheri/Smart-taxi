using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.DataModels
{
    public class DriverReqRequestModel
    {

        public DriverReqRequestModel()
        {
        }

        public int SLongitude { get; set; }
        public int SLatitude { get; set; }
        public string DriverId { get; set; }

        public DriverReqRequestModel(int sLongitude, int sLatitude, string driverId)
        {
            SLongitude = sLongitude;
            SLongitude = sLatitude;
            DriverId = driverId;
        }
    }
}
