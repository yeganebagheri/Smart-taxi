using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Auth.Models
{
    public class LoginDto
    {
       // public Tokens Tokens { get; set; }
        public User User { get; set; }
        //public List<Core.Entities.Role> Roles { get; set; }
    }
}
