using Microsoft.EntityFrameworkCore;
using GestionPuntosAPI.Models;
using BCrypt.Net;

namespace GestionPuntosAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Datos de prueba (seeding)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FirstName = "Admin",
                    Points = 1000,
                    IsAdmin = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = 2,
                    Email = "user@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    FirstName = "Usuario",
                    Points = 500,
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}