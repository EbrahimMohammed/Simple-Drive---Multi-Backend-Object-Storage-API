using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Business.BlobTrackingsRepository;
using Business.UserContextService;
using Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.StorageBackendsServices;
using Services.StorageOptions;

public class S3StorageService : IStorageService
{
    private readonly HttpClient _httpClient;
    private readonly S3Options _options;
    private readonly IBlobTrackingsRepository _bloblTrackingsRepository;
    private readonly IUserContextService _userContextService;

    public S3StorageService(IHttpClientFactory httpClientFactory,
        IOptions<S3Options> options,
        ILogger<S3StorageService> logger,
        IBlobTrackingsRepository blobTrackingsRepository,
        IUserContextService userContextService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _options = options.Value;
        _bloblTrackingsRepository = blobTrackingsRepository;
        _userContextService = userContextService;
    }

    public async Task StoreBlobAsync(string id, byte[] data)
    {
        string url = BuildUrl(id);

        using var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = new ByteArrayContent(data)
        };
        request.Content.Headers.Add("Content-Type", "application/octet-stream");

        SignRequest(request, HttpMethod.Put.Method, id);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowException("Failed to store blob", response);
        }

        var blobTracking = new BlobTracking
        {
            Id = id,
            Size = data.Length,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _userContextService.UserId,
            StorageType = StorageTypes.S3
        };
        await _bloblTrackingsRepository.Add(blobTracking);

    }

    public async Task<byte[]> GetBlobAsync(string id)
    {
        string url = BuildUrl(id);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        SignRequest(request, HttpMethod.Get.Method, id);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            await ThrowException("Failed to retrieve blob", response);
        }

        return await response.Content.ReadAsByteArrayAsync();
    }

    private void SignRequest(HttpRequestMessage request, string method, string key)
    {
        string service = "s3";
        string host = $"{_options.BucketName}.s3.{_options.Region}.amazonaws.com";
        string datetime = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
        string date = DateTime.UtcNow.ToString("yyyyMMdd");

        request.Headers.Add("Host", host);
        request.Headers.Add("x-amz-date", datetime);

        string payloadHash = method == HttpMethod.Put.Method
            ? Hash(request.Content.ReadAsByteArrayAsync().Result) 
            : "UNSIGNED-PAYLOAD";
        request.Headers.Add("x-amz-content-sha256", payloadHash);

        var canonicalRequest = $"{method}\n/{key}\n\nhost:{host}\nx-amz-content-sha256:{payloadHash}\nx-amz-date:{datetime}\n\nhost;x-amz-content-sha256;x-amz-date\n{payloadHash}";
        var scope = $"{date}/{_options.Region}/{service}/aws4_request";
        var stringToSign = $"AWS4-HMAC-SHA256\n{datetime}\n{scope}\n{Hash(Encoding.UTF8.GetBytes(canonicalRequest))}";

        var signingKey = GetSignatureKey(_options.SecretKey, date, _options.Region, service);
        var signature = HmacSHA256(signingKey, stringToSign);

        var signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLower();
        var authHeader = $"AWS4-HMAC-SHA256 Credential={_options.AccessKey}/{scope}, SignedHeaders=host;x-amz-content-sha256;x-amz-date, Signature={signatureHex}";

        request.Headers.TryAddWithoutValidation("Authorization", authHeader);
    }

    private async Task ThrowException(string message, HttpResponseMessage response)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"{message}. Status Code: {response.StatusCode}, Response: {responseBody}");
    }

    private string BuildUrl(string id) =>
        $"https://{_options.BucketName}.s3.{_options.Region}.amazonaws.com/{id}";

    private byte[] GetSignatureKey(string key, string date, string region, string service)
    {
        byte[] kDate = HmacSHA256(Encoding.UTF8.GetBytes("AWS4" + key), date);
        byte[] kRegion = HmacSHA256(kDate, region);
        byte[] kService = HmacSHA256(kRegion, service);
        return HmacSHA256(kService, "aws4_request");
    }

    private byte[] HmacSHA256(byte[] key, string data) =>
        new HMACSHA256(key).ComputeHash(Encoding.UTF8.GetBytes(data));

    private string Hash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(data);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
