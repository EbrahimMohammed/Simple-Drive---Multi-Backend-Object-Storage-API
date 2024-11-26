using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StorageOptions
{
    public class LocalOptions
    {
        [Required]

        public string StoragePath { get; set; }

    }
}
