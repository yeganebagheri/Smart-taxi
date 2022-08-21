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

        public string SLongitude { get; set; }
        public string SLatitude { get; set; }
        public string DriverId { get; set; }

       
    }
}
