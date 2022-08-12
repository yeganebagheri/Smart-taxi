using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Driver_req
    {
       
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public Guid DriverId { get; set; }
        public string ConectionId { get; set; }
        public bool IsReady { get; set; }
    }
}
