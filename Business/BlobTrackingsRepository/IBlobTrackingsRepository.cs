using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.BlobTrackingsRepository
{
    public interface IBlobTrackingsRepository
    {
        public Task Add(BlobTracking blobTracking);

        public Task<BlobTracking> GetById(string id);

        public Task<bool> BlobIdExists(string id);
    }
}
