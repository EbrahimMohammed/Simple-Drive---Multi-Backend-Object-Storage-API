using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StorageOptions
{
    public class FtpOptions
    {
        [Required]
        public string ServerUrl { get; set; }
        [Required]

        public string Username { get; set; }
        [Required]

        public string Password { get; set; }
        [Required]

        public string BasePath { get; set; }
    }
}
