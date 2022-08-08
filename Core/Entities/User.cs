using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public bool gender { get; set; }
        public string username { get; set; }
        public string phoneNo { get; set; }
        public string Password { get; set; }
        public bool role { get; set; }
    }
}
