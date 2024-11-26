using System.Net;
using System.Text;
using Business.BlobTrackingsRepository;
using Business.UserContextService;
using Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.StorageBackendsServices;
using Services.StorageOptions;

public class FtpStorageService : IStorageService
{
    private readonly FtpOptions _options;
    private readonly IUserContextService _userContextService;
    private readonly IBlobTrackingsRepository _blobTrackingRepository;

    public FtpStorageService(IOptions<FtpOptions> options,
        IUserContextService userContextService,
        IBlobTrackingsRepository blobTrackingsRepository)
    {
        _options = options.Value;
        _userContextService = userContextService;
        _blobTrackingRepository = blobTrackingsRepository;
    }

    public async Task StoreBlobAsync(string id, byte[] data)
    {
        var uri = BuildUri(id);

        var request = (FtpWebRequest)WebRequest.Create(uri);
        request.Method = WebRequestMethods.Ftp.UploadFile;
        request.Credentials = new NetworkCredential(_options.Username, _options.Password);
        request.UseBinary = true;

        using (var requestStream = await request.GetRequestStreamAsync())
        {
            await requestStream.WriteAsync(data, 0, data.Length);
        }

        using var response = (FtpWebResponse)await request.GetResponseAsync();

        if (response.StatusCode != FtpStatusCode.ClosingData)
        {
            throw new Exception($"Failed to upload blob '{id}'. FTP response: {response.StatusDescription}");
        }

        var blobTracking = new BlobTracking
        {
            Id = id,
            PathOrLocation = uri,
            StorageType = StorageTypes.FTP,
            Size = data.Length,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userContextService.UserId
        };

        await _blobTrackingRepository.Add(blobTracking);


    }

    public async Task<byte[]> GetBlobAsync(string id)
    {
        var uri = BuildUri(id);


        var request = (FtpWebRequest)WebRequest.Create(uri);
        request.Method = WebRequestMethods.Ftp.DownloadFile;
        request.Credentials = new NetworkCredential(_options.Username, _options.Password);
        request.UseBinary = true;

        using var response = (FtpWebResponse)await request.GetResponseAsync();
        using var responseStream = response.GetResponseStream();
        using var memoryStream = new MemoryStream();

        if (responseStream != null)
        {
            await responseStream.CopyToAsync(memoryStream);
        }

        return memoryStream.ToArray();

    }

    private string BuildUri(string id)
    {
        if (string.IsNullOrWhiteSpace(_options.ServerUrl))
            throw new ArgumentException("ServerUrl must be provided in the FTP options.");

        if (string.IsNullOrWhiteSpace(_options.BasePath))
            throw new ArgumentException("BasePath must be provided in the FTP options.");

        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Blob ID cannot be null or empty.");

        var sanitizedBasePath = _options.BasePath.Trim('/'); // Remove leading/trailing slashes
        var sanitizedId = Uri.EscapeDataString(id);         // Escape special characters in ID

        return $"{_options.ServerUrl.TrimEnd('/')}/{sanitizedBasePath}/{sanitizedId}";
    }

}
