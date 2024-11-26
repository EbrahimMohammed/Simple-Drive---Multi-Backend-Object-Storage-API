using Services.StorageBackendsServices;

namespace API.Services
{
    public class StorageServiceFactory : IStorageServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public StorageServiceFactory(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public IStorageService CreateStorageService()
        {
            var backend = _configuration["Storage:Backend"];
            return backend switch
            {
                StorageTypes.S3 => _serviceProvider.GetRequiredService<S3StorageService>(),
                StorageTypes.Database => _serviceProvider.GetRequiredService<DatabaseStorageService>(),
                StorageTypes.Local => _serviceProvider.GetRequiredService<LocalStorageService>(),
                StorageTypes.FTP => _serviceProvider.GetRequiredService<FtpStorageService>(),
                _ => throw new InvalidOperationException($"Unsupported storage backend: {backend}")
            };
        }
    }

}
