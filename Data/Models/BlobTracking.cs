using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class BlobTracking
    {
        public string Id { get; set; }

        public string? PathOrLocation { get; set; }

        public long Size { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public int CreatedBy { get; set; }

        public string StorageType { get; set; }


    }
}
