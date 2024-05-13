using Microsoft.EntityFrameworkCore;

namespace MassTransitExample;

public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}
