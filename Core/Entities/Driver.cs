using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("Driver")]
    public class Driver
    {
        [Key]
        public Guid Id { get; set; }
        public Guid userId { get; set; }
        public string car { get; set; }
        public string carId { get; set; }
    }
}
