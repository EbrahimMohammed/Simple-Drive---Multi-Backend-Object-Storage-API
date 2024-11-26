using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Business.UsersRepository;
using API.Services;
using Services.StorageOptions;
using Business.BlobTrackingsRepository;
using API.Validators;
using Services.StorageBackendsServices;
using Business.UserContextService;
namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddFluentValidationServices(this IServiceCollection services)
        {
            // Register FluentValidation and automatically scan for validators
            services.AddControllers()
                    .AddFluentValidation(fv =>
                    {
                        // Automatically register all validators in the current application domain
                        fv.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                    });

            return services;
        }


        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlobTrackingsRepository, BlobTrackingsRepository>();
            return services;

        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddOptions<LocalOptions>()
                .Bind(configuration.GetSection("Storage:Local"))
                .ValidateDataAnnotations(); 

            services.AddOptions<S3Options>()
                .Bind(configuration.GetSection("Storage:S3"))
                .ValidateDataAnnotations(); 
            services.AddOptions<FtpOptions>()
                .Bind(configuration.GetSection("Storage:FTP"))
                .ValidateDataAnnotations(); 

            services.AddScoped<LocalStorageService>();
            services.AddScoped<S3StorageService>();
            services.AddScoped<DatabaseStorageService>();
            services.AddScoped<FtpStorageService>();
            services.AddScoped<IStorageServiceFactory, StorageServiceFactory>();
            services.AddScoped(provider =>
            {
                var factory = provider.GetRequiredService<IStorageServiceFactory>();
                return factory.CreateStorageService();
            });
            services.AddHttpClient();

            return services;
        }



    }
}
