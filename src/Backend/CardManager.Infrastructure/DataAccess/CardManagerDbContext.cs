using CardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardManager.Infrastructure.DataAccess
{
    public class CardManagerDbContext : DbContext
    {
        public CardManagerDbContext(DbContextOptions DbOptions) : base(DbOptions) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CardManagerDbContext).Assembly);
        }
        
    }
}
