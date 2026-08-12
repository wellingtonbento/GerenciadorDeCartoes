using CardManager.Domain.Identity;
using CardManager.Domain.Repositories;
using CardManager.Domain.Repositories.Card;
using CardManager.Domain.Repositories.Transaction;
using CardManager.Domain.Repositories.User;
using CardManager.Domain.Security.Tokens;
using CardManager.Infrastructure.DataAccess;
using CardManager.Infrastructure.DataAccess.Repositories;
using CardManager.Infrastructure.Extensions;
using CardManager.Infrastructure.Identity;
using CardManager.Infrastructure.Security.Tokens;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CardManager.Infrastructure
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddRepositories(services);
            AddJwtToken(services, configuration);

            services.AddScoped<ILoggedUser, LoggedUser>();

            if (configuration.IsUniTestEnviromente())
                return;

            AddDbContext(services, configuration);
            AddFluentMigrator(services, configuration);
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
            services.AddScoped<IUserUpdateRepository, UserRepository>();
            services.AddScoped<IUserDeleteRepository, UserRepository>();

            services.AddScoped<ICardWriteRepository, CardRepository>();
            services.AddScoped<ICardReadRepository, CardRepository>();
            services.AddScoped<ICardUpdateRepository, CardRepository>();

            services.AddScoped<ITransactionWriteRepository, TransactionRepository>();
            services.AddScoped<ITransactionReadRepository, TransactionRepository>();
            services.AddScoped<ITransactionUpdateRepository, TransactionRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private static void AddJwtToken(IServiceCollection services, IConfiguration configuration)
        {
            var expirationTokenInMunites = configuration.GetValue<uint>("Jwt:ExpirationTokenInMunites");
            var signinKey = configuration.GetValue<string>("Jwt:SigninKey")!;

            services.AddScoped<ITokenGenerator>(provider =>
            {
                return new JwtToken(expirationTokenInMunites, signinKey);
            });
        }

        private static void AddFluentMigrator(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.ConnectionString();
            services.AddFluentMigratorCore().ConfigureRunner(options =>
            {
                options.AddMySql5()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(Assembly.Load("CardManager.Infrastructure")).For.All();
            });
        }
    }
}
