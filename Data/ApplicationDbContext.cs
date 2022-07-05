using ContentFactory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ContentFactory.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json");
            var configuration = builder.Build();

            optionsBuilder.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);

        }
        public DbSet<Txt> Txts { get; set; }
        public DbSet<User> AppUsers { get; set; }
        public DbSet<TempUser> TempUsers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ModelStyle> ModelStyles { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderFile> OrderFiles { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<CatalogImage> CatalogImages { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelImage> ModelImages { get; set; }

        protected override async void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
             .Property(b => b.RegDate)
             .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Model>()
             .Property(b => b.WorkDate)
             .HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Catalog>()
             .Property(b => b.isVisible)
             .HasDefaultValueSql("((0))");

            //modelBuilder.Entity<User>(b =>
            //{
            //    b.HasKey(e => e.IdNumber);
            //    b.Property(e => e.IdNumber).ValueGeneratedOnAdd();

            //});



            base.OnModelCreating(modelBuilder);
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    }
}