using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.DataModels
{
    public class PreTrip
    {
        public Guid Id { get; set; }
        public Guid SubPreTrip1Id { get; set; }
        public Guid? SubPreTrip2Id { get; set; }
        public Guid? SubPreTrip3Id { get; set; }
        public bool IsProcessed { get; set; }

    }
}
