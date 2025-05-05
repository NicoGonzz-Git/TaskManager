using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager_API.Models.Domain;

namespace TaskManager_API.Data
{
    public class TaskContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public TaskContext(DbContextOptions<TaskContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>()
                .ToContainer(_configuration["CosmosDb:Containers:Tasks"] ?? "tasks")
                .HasPartitionKey(t => t.Id)
                .HasKey(t => t.Id);
        }
    }
}