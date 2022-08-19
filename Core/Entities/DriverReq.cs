using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("DriverReq")]
    public class DriverReq
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public double SLongitude { get; set; }
        public double SLatitude { get; set; }
        //public string ConectionId { get; set; }
        public bool IsReady { get; set; }
    }
}
