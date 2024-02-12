using Microsoft.EntityFrameworkCore;
using UAMangaAPI.Models;

namespace UAMangaAPI.Data
{
    public class UAMangaAPIDbContext : DbContext
    {
        public UAMangaAPIDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manga>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<User>()
                .HasNoKey();


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Manga> Mangas { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
