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
        public Guid SubPreTrip1Id { get; set; }
        public Guid? SubPreTrip2Id { get; set; }
        public Guid? SubPreTrip3Id { get; set; }
        public bool IsProcessed { get; set; }

    }
}
