using BioInfoProject.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace BioInfoProject.Server.Data;

public class BioInfoDbContext : DbContext
{
    public BioInfoDbContext(DbContextOptions<BioInfoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Dataset> Datasets => Set<Dataset>();

    public DbSet<EpidemiologicalRecord> EpidemiologicalRecords => Set<EpidemiologicalRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Dataset>(entity =>
        {
            entity.ToTable("Datasets");

            entity.HasKey(dataset => dataset.Id);

            entity.Property(dataset => dataset.FileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(dataset => dataset.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(dataset => dataset.UploadedAt)
                .IsRequired();

            entity.HasMany(dataset => dataset.Records)
                .WithOne(record => record.Dataset)
                .HasForeignKey(record => record.DatasetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EpidemiologicalRecord>(entity =>
        {
            entity.ToTable("EpidemiologicalRecords");

            entity.HasKey(record => record.Id);

            entity.Property(record => record.CountryCode)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(record => record.Country)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(record => record.WhoRegion)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(record => record.DatasetId);
            entity.HasIndex(record => new { record.DatasetId, record.Country });
            entity.HasIndex(record => new { record.DatasetId, record.WhoRegion });
            entity.HasIndex(record => new { record.DatasetId, record.DateReported });
        });
    }
}
