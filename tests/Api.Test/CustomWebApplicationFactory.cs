using CardManager.Domain.Entities;
using CardManager.Infrastructure.DataAccess;
using CoreTestUtilities.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test")
                .ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<CardManagerDbContext>));
                    if (descriptor is not null)
                        services.Remove(descriptor);

                    var provider = services.AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                    services.AddDbContext<CardManagerDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                        options.UseInternalServiceProvider(provider);
                    });
                });
        }

        public async Task<(CardManager.Domain.Entities.User user, string password)> SeedUserAsync()
        {
            (var user, var password) = UserBuilder.Build();

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return (user, password);
        }

        public async Task<CardManager.Domain.Entities.Card> SeedCardAsync(CardManager.Domain.Entities.User user, decimal creditLimit, decimal creditBalance)
        {
            var card = CardBuilder.Build();
            card.UserId = user.Id;
            card.CreditLimit = creditLimit;
            card.CreditBalance = creditBalance;

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            await dbContext.Cards.AddAsync(card);
            await dbContext.SaveChangesAsync();

            return card;
        }

        public async Task<CardManager.Domain.Entities.Card> SeedCardAsync(CardManager.Domain.Entities.User user)
        {
            var card = CardBuilder.Build();
            card.UserId = user.Id;

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CardManagerDbContext>();

            await dbContext.Cards.AddAsync(card);
            await dbContext.SaveChangesAsync();

            return card;
        }
    }
}
