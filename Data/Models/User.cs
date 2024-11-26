using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string UserName { get; set; } 
        public string PasswordHash { get; set; } 
        public DateTime CreatedAt { get; set; } 
    }

}
