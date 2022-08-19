using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.DataModels
{
    public class ProfileDto
    {
        public User user { get; set; }
        public Passenger passenger { get; set; }
        public Driver driver { get; set; }
    }
}
