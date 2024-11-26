using Services.StorageBackendsServices;

namespace API.Services
{
    public interface IStorageServiceFactory
    {
        IStorageService CreateStorageService();
    }
}
