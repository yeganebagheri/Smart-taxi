using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.DataModels
{
    [Table("PreTrip")]
    public class PreTrip
    {
        public Guid Id { get; set; }
        public bool IsProcessed { get; set; }

        //1
        public double SLongitude1 { get; set; }
        public double SLatitude1 { get; set; }
        public double DLongitude1 { get; set; }
        public double DLatitude1 { get; set; }
        public string username1 { get; set; }
        public string phoneNo1 { get; set; }

        //2
        public double? SLongitude2 { get; set; }
        public double? SLatitude2 { get; set; }
        public double? DLongitude2 { get; set; }
        public double? DLatitude2 { get; set; }
        public string username2 { get; set; }
        public string phoneNo2 { get; set; }

        //3
        public double? SLongitude3 { get; set; }
        public double? SLatitude3 { get; set; }
        public double? DLongitude3 { get; set; }
        public double? DLatitude3 { get; set; }
        public string username3 { get; set; }
        public string phoneNo3 { get; set; }

    }
}
