using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager_API.Models.Domain;

namespace TaskManager_API.Data
{
    public class UserContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public UserContext(DbContextOptions<UserContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToContainer(_configuration["CosmosDb:Containers:Users"] ?? "users")
                .HasPartitionKey(u => u.Id)
                .HasKey(u => u.Id);
        }
    }
}