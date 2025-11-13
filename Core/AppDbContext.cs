using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core;

public sealed class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<NasaDataset> NasaDbSet { get; init; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = _configuration.GetConnectionString("Postgres");
        optionsBuilder.UseNpgsql(
            connection,
            builder =>
            {
                builder.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
            });
        
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NasaDataset>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Date);
            
            entity.HasIndex(e => e.Date);
        
            entity.OwnsOne(e => e.Geolocation, g =>
            {
                g.Property(geo => geo.Type).HasColumnName("GeoLocationType");
                g.Property(geo => geo.Coordinates).HasColumnName("GeoCoordinates");
            });
        });
    }
}