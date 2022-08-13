using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.DataModels
{
    [Table("SubPreTrip")]
    public class SubPreTrip
    {
        public Guid Id { get; set; }
        public double SLongitude { get; set; }
        public double SLatitude { get; set; }
        public double DLongitude { get; set; }
        public double DLatitude { get; set; }
        public string username { get; set; }
        public string phoneNo { get; set; }
    }
}
