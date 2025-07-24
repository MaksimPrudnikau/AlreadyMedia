using Core;
using Microsoft.EntityFrameworkCore;

namespace AlreadyMedia.Contexts;

public sealed class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<NasaDataset> NasaDbSet { get; init; }
    
    public AppDbContext(DbContextOptions<AppDbContext> context, IConfiguration configuration) : base(context)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Postgres"));
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NasaDataset>(entity =>
        {
            entity.HasKey(e => e.Id);
        
            entity.OwnsOne(e => e.Geolocation, g =>
            {
                g.Property(geo => geo.Type).HasColumnName("GeoLocationType");
                g.Property(geo => geo.Coordinates).HasColumnName("GeoCoordinates");
            });
        });
    }
}