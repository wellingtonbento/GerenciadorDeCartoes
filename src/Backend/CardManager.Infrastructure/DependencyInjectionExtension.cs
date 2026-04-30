using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.User;
using CardManager.Infrastructure.DataAccess;
using CardManager.Infrastructure.DataAccess.Repositories;
using CardManager.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CardManager.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddRepositories(services);
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 40));

            services.AddDbContext<CardManagerDbContext>(dbContextOptions =>
            {
                dbContextOptions.UseMySql(connectionString, serverVersion);
            });
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserWriteRepository, UserRepository>();
            services.AddScoped<IUserReadRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
