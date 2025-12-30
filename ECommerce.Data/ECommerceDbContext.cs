using System;
using ECommerce.Entity.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ECommerce.Data;

public class ECommerceDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
    {

    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted);




        modelBuilder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<ProductCategory>().HasKey(x => new { x.ProductId, x.CategoryId });





        #region Rol Bilgileri
        var appRoles = new AppRole[]
        {
            new AppRole { Id = "dd66d9d0-5aac-42a3-bd53-12ac0574cf1c", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "c4addbe1-adbc-4002-90dd-c4c2391eafb5", Description="Yönetici Rolü" },
            new AppRole { Id = "e676b617-bec3-4b92-a746-dfc5043ebe08", Name = "User", NormalizedName = "USER", ConcurrencyStamp = "ef25dd62-cbbc-45aa-8405-ffa6d8926664", Description="Kullanıcı Rolü" }
        };
        modelBuilder.Entity<AppRole>().HasData(appRoles);
        #endregion

        #region Kullanıcı Bilgileri
        var hasher = new PasswordHasher<AppUser>();
        var appUsers = new List<AppUser>();

        var appUser1 = new AppUser { Id = "819bee56-04d5-4ba7-8bd4-109d7607af95", FirstName = "Deniz", LastName = "Kerem", Email = "denizkerem@example.com", EmailConfirmed = true, UserName = "denizkerem", NormalizedEmail = "DENIZKEREM@EXAMPLE.COM", NormalizedUserName = "DENIZKEREM", ConcurrencyStamp = "087729db-48a9-434f-90a0-4fe1af527ff8", SecurityStamp = "a9aa84f6-6a60-45de-9070-40d23d2f403b" };

        var appUser2 = new AppUser { Id = "6c5e6042-9145-42cb-8220-e4aab7ea0cdb", FirstName = "Selin", LastName = "Dağ", Email = "selindag@example.com", EmailConfirmed = true, UserName = "selindag", NormalizedEmail = "SELINDAG@EXAMPLE.COM", NormalizedUserName = "SELINDAG", ConcurrencyStamp = "2f9ab541-0444-4ed4-beb6-10edae0e65bf", SecurityStamp = "8d096aa5-6071-4e16-8c8e-ecf542ec361d", };

        var appUser3 = new AppUser { Id = "a8c5e701-43ad-4949-ac22-32385e7cfd88", FirstName = "Ferda", LastName = "Can", Email = "ferdacan@example.com", EmailConfirmed = true, UserName = "ferdacan", NormalizedEmail = "FERDACAN@EXAMPLE.COM", NormalizedUserName = "FERDACAN", ConcurrencyStamp = "cfe87fd5-71af-41b1-98fe-8f16123a4966", SecurityStamp = "c1eeafd8-8177-4e60-ba9b-d67ab8f53be8" };


        appUser1.PasswordHash = hasher.HashPassword(appUser1, "Qwe123.,");
        appUser2.PasswordHash = hasher.HashPassword(appUser2, "Qwe123.,");
        appUser3.PasswordHash = hasher.HashPassword(appUser3, "Qwe123.,");

        modelBuilder.Entity<AppUser>().HasData(appUser1, appUser2, appUser3);
        #endregion

        #region Kullanıcı/Rol Atamaları
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>
            {
                UserId = "819bee56-04d5-4ba7-8bd4-109d7607af95",
                RoleId = "dd66d9d0-5aac-42a3-bd53-12ac0574cf1c"
            },
            new IdentityUserRole<string>
            {
                UserId = "6c5e6042-9145-42cb-8220-e4aab7ea0cdb",
                RoleId = "e676b617-bec3-4b92-a746-dfc5043ebe08"
            },
            new IdentityUserRole<string>
            {
                UserId = "a8c5e701-43ad-4949-ac22-32385e7cfd88",
                RoleId = "e676b617-bec3-4b92-a746-dfc5043ebe08"
            }
        );
        #endregion

        #region Kategori Bilgileri
        var categories = new Category[]
        {
            new Category
            {
                Id = 1,
                Name = "Akıllı Telefonlar",
                Description = "Güncel akıllı telefon modelleri",
                CreatedAt = new DateTime(2025,11,10),
                ModifiedAt = new DateTime(2025,11,10)
            },
            new Category
            {
                Id = 2,
                Name = "Aksesuarlar",
                Description = "Telefon aksesuarları ve tamamlayıcı ürünler",
                CreatedAt = new DateTime(2025,11,11),
                ModifiedAt = new DateTime(2025,11,11)
            },
            new Category
            {
                Id = 3,
                Name = "Elektronik",
                Description = "Genel elektronik ürünler",
                CreatedAt = new DateTime(2025,11,12),
                ModifiedAt = new DateTime(2025,11,12)
            }
        };

        modelBuilder.Entity<Category>().HasData(categories);
        #endregion

        #region Ürün Bilgileri
        var products = new Product[]
        {
            new Product { Id=1, Name="iPhone 16 Pro", Price=72000, StockQuantity=12, CreatedAt=new DateTime(2025,11,19), ModifiedAt=new DateTime(2025,11,19) },
            new Product { Id=2, Name="iPhone 16", Price=64000, StockQuantity=20, CreatedAt=new DateTime(2025,11,19), ModifiedAt=new DateTime(2025,11,19) },
            new Product { Id=3, Name="Samsung Galaxy S25 Ultra", Price=68000, StockQuantity=15, CreatedAt=new DateTime(2025,11,18), ModifiedAt=new DateTime(2025,11,18) },
            new Product { Id=4, Name="Xiaomi 14 Ultra", Price=52000, StockQuantity=30, CreatedAt=new DateTime(2025,11,17), ModifiedAt=new DateTime(2025,11,17) },
            new Product { Id=5, Name="Google Pixel 9 Pro", Price=58000, StockQuantity=18, CreatedAt=new DateTime(2025,11,16), ModifiedAt=new DateTime(2025,11,16) },

            new Product { Id=6, Name="Apple AirPods Pro 3", Price=12000, StockQuantity=50, CreatedAt=new DateTime(2025,11,15), ModifiedAt=new DateTime(2025,11,15) },
            new Product { Id=7, Name="Samsung Galaxy Buds 3 Pro", Price=9000, StockQuantity=60, CreatedAt=new DateTime(2025,11,14), ModifiedAt=new DateTime(2025,11,14) },
            new Product { Id=8, Name="Anker PowerCore 30000 mAh", Price=3500, StockQuantity=100, CreatedAt=new DateTime(2025,11,13), ModifiedAt=new DateTime(2025,11,13) },
            new Product { Id=9, Name="Baseus GaN 65W Şarj Cihazı", Price=2500, StockQuantity=80, CreatedAt=new DateTime(2025,11,11), ModifiedAt=new DateTime(2025,11,11) },
            new Product { Id=10, Name="Logitech MX Master 4", Price=6000, StockQuantity=40, CreatedAt=new DateTime(2025,11,10), ModifiedAt=new DateTime(2025,11,10) }
        };

        modelBuilder.Entity<Product>().HasData(products);
        #endregion

        #region Ürün-Kategori Bağlantıları
        var productCategories = new ProductCategory[]
        {
            // İlk 5 ürün (Id 1-5) -> Kategori 1 ve 3
            new ProductCategory { ProductId = 1, CategoryId = 1 },
            new ProductCategory { ProductId = 1, CategoryId = 3 },

            new ProductCategory { ProductId = 2, CategoryId = 1 },
            new ProductCategory { ProductId = 2, CategoryId = 3 },

            new ProductCategory { ProductId = 3, CategoryId = 1 },
            new ProductCategory { ProductId = 3, CategoryId = 3 },

            new ProductCategory { ProductId = 4, CategoryId = 1 },
            new ProductCategory { ProductId = 4, CategoryId = 3 },

            new ProductCategory { ProductId = 5, CategoryId = 1 },
            new ProductCategory { ProductId = 5, CategoryId = 3 },

            // Sonraki 5 ürün (Id 6-10) -> Kategori 1, 2 ve 3
            new ProductCategory { ProductId = 6, CategoryId = 1 },
            new ProductCategory { ProductId = 6, CategoryId = 2 },
            new ProductCategory { ProductId = 6, CategoryId = 3 },

            new ProductCategory { ProductId = 7, CategoryId = 1 },
            new ProductCategory { ProductId = 7, CategoryId = 2 },
            new ProductCategory { ProductId = 7, CategoryId = 3 },

            new ProductCategory { ProductId = 8, CategoryId = 1 },
            new ProductCategory { ProductId = 8, CategoryId = 2 },
            new ProductCategory { ProductId = 8, CategoryId = 3 },

            new ProductCategory { ProductId = 9, CategoryId = 1 },
            new ProductCategory { ProductId = 9, CategoryId = 2 },
            new ProductCategory { ProductId = 9, CategoryId = 3 },

            new ProductCategory { ProductId = 10, CategoryId = 1 },
            new ProductCategory { ProductId = 10, CategoryId = 2 },
            new ProductCategory { ProductId = 10, CategoryId = 3 }
        };

        modelBuilder.Entity<ProductCategory>().HasData(productCategories);
        #endregion


    }
}
