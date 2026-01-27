using Microsoft.EntityFrameworkCore;
using OSRSData.Core.Entities;

namespace OSRSData.DAL;

public class OSRSDbContext : DbContext
{
    public OSRSDbContext(DbContextOptions<OSRSDbContext> options) : base(options)
    {
    }

    public DbSet<LogEntry> LogEntries { get; set; } = null!;
    public DbSet<LootRecord> LootRecords { get; set; } = null!;
    public DbSet<LootItem> LootItems { get; set; } = null!;
    public DbSet<DeathRecord> DeathRecords { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<LogEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Player).HasMaxLength(255);
            entity.Property(e => e.Type).HasConversion<string>();
            entity.Property(e => e.IpAddress).HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasMaxLength(1024);
            entity.Property(e => e.ReceivedAt).IsRequired();
            
            entity.HasOne(e => e.LootRecord)
                .WithOne()
                .HasForeignKey<LogEntry>(e => e.LootRecordId)
                .IsRequired(false);

            entity.HasOne(e => e.DeathRecord)
                .WithOne()
                .HasForeignKey<LogEntry>(e => e.DeathRecordId)
                .IsRequired(false);
        });

        modelBuilder.Entity<DeathRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Killer).HasMaxLength(255);
        });

        modelBuilder.Entity<LootRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Source).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<LootItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            
            entity.HasOne(e => e.LootRecord)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.LootRecordId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
