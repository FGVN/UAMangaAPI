using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using UAMangaAPI.Models;

namespace UAMangaAPI.Data
{
    public class UAMangaAPIDbContext : DbContext
    {
        public UAMangaAPIDbContext(DbContextOptions<UAMangaAPIDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manga>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().HasKey(u => u.Id);

            var listConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
            );

            modelBuilder.Entity<User>()
                .Property(x => x.Wishlist)
                .HasConversion(listConverter);

            modelBuilder.Entity<User>()
                .Property(x => x.Own)
                .HasConversion(listConverter);


            /*modelBuilder.Entity<UserManga>().HasKey(um => new { um.UserId, um.MangaId });

            modelBuilder.Entity<UserManga>()
                .HasOne(um => um.User)
                .WithMany(u => u.WishlistMangas)
                .HasForeignKey(um => um.UserId);

            modelBuilder.Entity<UserManga>()
                .HasOne(um => um.Manga)
                .WithMany(m => m.Users)
                .HasForeignKey(um => um.MangaId);*/

            base.OnModelCreating(modelBuilder);
        }

        //public DbSet<UserManga> UserMangas { get; set; } // Add this DbSet for the join table

        public DbSet<Manga> Mangas { get; set; }
        public DbSet<User> Users { get; set; }
        public object JsonConvert { get; private set; }
    }
}
