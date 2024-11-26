using API.Dtos;
using Business.BlobTrackingsRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.StorageBackendsServices;

namespace API.Controllers
{
    [Route("/v1/api/blobs")]
    [ApiController]
    [Authorize]
    public class BlobsController : ControllerBase
    {
        private readonly IStorageService _storageService;
        private readonly IBlobTrackingsRepository _blobTrackingRepository;

        public BlobsController(IStorageService storageService, IBlobTrackingsRepository blobTrackingsRepository)
        {
            _storageService = storageService;
            _blobTrackingRepository = blobTrackingsRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UploadBlob([FromBody] CreateBlobRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            byte[] decodedData;
            try
            {
                decodedData = Convert.FromBase64String(request.Data);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid Base64 string in the 'data' field.");
            }

            if(await _blobTrackingRepository.BlobIdExists(request.Id))
            {
                return Conflict($"A blob with the ID '{request.Id}' already exists.");
            }

            // Call the storage service
            await _storageService.StoreBlobAsync(request.Id, decodedData);

            return Ok("Blob uploaded successfully.");
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlob(string id)
        {
            var bloblTracking = await _blobTrackingRepository.GetById(id);
            if (bloblTracking == null)
            {
                return NotFound();
            }

            var blobData = await _storageService.GetBlobAsync(id);
            if (blobData == null)
            {
                return NotFound($"Blob with ID '{id}' not found.");
            }
       
            var response = new BlobResponse
            {
                Id = id,
                Data = Convert.ToBase64String(blobData), 
                Size = bloblTracking.Size,
                CreatedAt = bloblTracking.CreatedAt
            };

            return Ok(response);
        }
    }
}
