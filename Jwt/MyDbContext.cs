using Jwt.Models;
using Microsoft.EntityFrameworkCore;

namespace Jwt
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
  
        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoaiModel> Loais { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            ModelBuilder modelBuilder = new ModelBuilder();
            modelBuilder.Entity<NguoiDung>(e => {
                e.HasKey(x => x.Id);
                e.Property(x => x.UserName).IsRequired().HasMaxLength(50);
                e.Property(x => x.Password).IsRequired().HasMaxLength(250);

            });
            modelBuilder.Entity<LoaiModel>(e =>
            {
                e.HasKey(x => x.MaLoai);
                e.Property(x => x.TenLoai).IsRequired().HasMaxLength(50);
            });
        }

    }
}
