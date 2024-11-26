using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class BlobData
    {
        public string Id { get; set; }

        public  byte[] Blob { get; set; }

        public BlobTracking BlobTracking { get; set; }
    }
}
