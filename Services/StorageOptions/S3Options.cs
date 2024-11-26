using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StorageOptions
{
    public class S3Options
    {
        [Required]

        public string AccessKey { get; set; }
        [Required]

        public string SecretKey { get; set; }
        [Required]

        public string BucketName { get; set; }
        [Required]

        public string Region { get; set; }
    }
}
