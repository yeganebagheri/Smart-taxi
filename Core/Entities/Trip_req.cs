using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("Trip_req")]
    public class Trip_req
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid LocationId { get; set; }
        public Guid passengerId { get; set; }
        public string firstPrice { get; set; }
        public int passesNum { get; set; }
    }
}
