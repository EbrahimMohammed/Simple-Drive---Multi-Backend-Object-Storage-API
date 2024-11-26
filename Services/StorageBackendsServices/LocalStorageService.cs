using Business.BlobTrackingsRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.StorageOptions;
using System.IO;
using System.Threading.Tasks;


namespace Services.StorageBackendsServices
{
    using Business.UserContextService;
    using Microsoft.Extensions.Options;

    public class LocalStorageService : IStorageService
    {
        private readonly LocalOptions _options;
        private readonly IBlobTrackingsRepository _blobTrackingsRepository;
        private readonly IUserContextService _userContextService;

        public LocalStorageService(IOptions<LocalOptions> options,
            IBlobTrackingsRepository blobTrackingsRepository,
            IUserContextService userContextService)
        {
            _options = options.Value;

            if (!Directory.Exists(_options.StoragePath))
            {
                Directory.CreateDirectory(_options.StoragePath);
            }

            _blobTrackingsRepository = blobTrackingsRepository;
            _userContextService = userContextService;
        }

        public async Task StoreBlobAsync(string id, byte[] data)
        {
            var filePath = GetFilePath(id);
            await File.WriteAllBytesAsync(filePath, data);

            var blobTracking = new BlobTracking
            {
                Id = id,
                PathOrLocation = filePath,
                StorageType = StorageTypes.Local,
                Size = data.Length,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _userContextService.UserId
            };

            await _blobTrackingsRepository.Add(blobTracking);
        }

        public async Task<byte[]> GetBlobAsync(string id)
        {
            var filePath = GetFilePath(id);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The requested blob does not exist.", id);
            }

            return await File.ReadAllBytesAsync(filePath);
        }

        private string GetFilePath(string id)
        {
            var sanitizedId = Path.GetInvalidFileNameChars().Aggregate(id, (current, c) => current.Replace(c, '_'));
            return Path.Combine(_options.StoragePath, sanitizedId);
        }
    }


}
