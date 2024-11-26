using Business.UserContextService;
using Data;
using Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.StorageBackendsServices
{
    public class DatabaseStorageService : IStorageService
    {
        private readonly AppDbContext _context;
        private readonly IUserContextService _userContext;

        public DatabaseStorageService(AppDbContext context, IUserContextService userContextService)
        {
            _context = context;
            _userContext = userContextService;
        }

        public async Task StoreBlobAsync(string id, byte[] data)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Blob ID cannot be null or empty.", nameof(id));

            if (data == null || data.Length == 0)
                throw new ArgumentException("Blob data cannot be null or empty.", nameof(data));

            var blob = new BlobData
            {
                Id = id,
                Blob = data,
                BlobTracking = new BlobTracking
                {
                    Id = id,
                    CreatedAt = DateTime.UtcNow,
                    Size = data.Length,
                    CreatedBy = _userContext.UserId,
                    StorageType = StorageTypes.Database
                }
            };

            try
            {
                _context.BlobData.Add(blob);
                await _context.SaveChangesAsync();
                //_logger.LogInformation($"Blob with ID '{id}' stored successfully.");
            }
            catch (Exception ex)
            {
              //  _logger.LogError(ex, $"Failed to store blob with ID '{id}'.");
                throw;
            }
        }

        public async Task<byte[]> GetBlobAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("Blob ID cannot be null or empty.", nameof(id));

            try
            {
                var blob = await _context.BlobData.FindAsync(id);
                if (blob == null)
                {
                   // _logger.LogWarning($"Blob with ID '{id}' not found.");
                    throw new KeyNotFoundException($"Blob with ID '{id}' not found.");
                }

                //_logger.LogInformation($"Blob with ID '{id}' retrieved successfully.");
                return blob.Blob;
            }
            catch (Exception ex)
            {
               // _logger.LogError(ex, $"Failed to retrieve blob with ID '{id}'.");
                throw;
            }
        }
    }
}
