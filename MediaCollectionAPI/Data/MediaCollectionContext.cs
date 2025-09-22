using Microsoft.EntityFrameworkCore;
using MediaCollectionAPI.Models;
using Npgsql;

namespace MediaCollectionAPI.Data
{
    public class MediaCollectionContext : DbContext
    {
        static MediaCollectionContext()
        {
            
        }
        
        public MediaCollectionContext(DbContextOptions<MediaCollectionContext> options)
            : base(options)
        {
        }

        public DbSet<MediaItem> MediaItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresEnum<MediaType>();
            
            modelBuilder.Entity<MediaItem>(entity =>
            {
                entity.ToTable("media_items");
                
                entity.Property(e => e.MediaType)
                    .HasColumnName("media_type")
                    .HasConversion<string>();

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasConversion<string>();
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("gen_random_uuid()");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .IsRequired();

                entity.Property(e => e.Platform)
                    .HasColumnName("platform");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating");

                entity.Property(e => e.Price)
                    .HasColumnName("price");

                entity.Property(e => e.PriceLastUpdated)
                    .HasColumnName("price_last_updated");

                entity.Property(e => e.IsFavorite)
                    .HasColumnName("is_favorite");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity");

                entity.Property(e => e.Barcode)
                    .HasColumnName("barcode");

                entity.Property(e => e.ReleaseYear)
                    .HasColumnName("release_year");

                entity.Property(e => e.Publisher)
                    .HasColumnName("publisher");

                entity.Property(e => e.Genre)
                    .HasColumnName("genre");

                entity.Property(e => e.Condition)
                    .HasColumnName("condition");

                entity.Property(e => e.Notes)
                    .HasColumnName("notes");

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");
            });
            
            modelBuilder.Entity<MediaItem>()
                .HasOne(m => m.User)
                .WithMany(u => u.MediaItems)
                .HasForeignKey(m => m.UserId);
        }
    }
}