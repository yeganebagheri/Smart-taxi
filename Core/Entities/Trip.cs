using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("Trip")]
    public class Trip
    {
        [Key]
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public bool isCancled { get; set; }
        public string price { get; set; }
    }
}
