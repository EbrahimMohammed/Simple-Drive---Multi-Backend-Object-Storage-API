using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StorageBackendsServices
{
    public interface IStorageService
    {
        public Task StoreBlobAsync(string id, byte[] data);
        public Task<byte[]> GetBlobAsync(string id);
    }
}
