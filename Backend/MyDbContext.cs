using Jwt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jwt
{
    public class MyDbContext : IdentityDbContext<ApplicationUser>
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
  
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoaiModel> Loais { get; set; }
        public DbSet<ProductModel> Products { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            ModelBuilder modelBuilder = new ModelBuilder();
            modelBuilder.Entity<LoaiModel>(e =>
            {
                e.HasKey(x => x.MaLoai);
                e.Property(x => x.TenLoai).IsRequired().HasMaxLength(50);
            });
            modelBuilder.Entity<ProductModel>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).IsRequired().HasMaxLength(100);
                e.Property(x => x.Price).IsRequired();
                e.HasOne<LoaiModel>(p => p.Loai)
                .WithMany()
                .HasForeignKey(p => p.MaLoai)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }

    }
}
