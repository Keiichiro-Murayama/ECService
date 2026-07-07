using ECService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<DepartmentEntity> Departments => Set<DepartmentEntity>();
        public DbSet<EmployeeEntity> Employees => Set<EmployeeEntity>();
        public DbSet<EmployeeAccountEntity> EmployeeAccounts => Set<EmployeeAccountEntity>();
        public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<ProductStockEntity> ProductStocks => Set<ProductStockEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ########### 社員関連 ###########
            modelBuilder.Entity<EmployeeEntity>(entity =>
            {
                //Unique設定
                entity.HasIndex(e => e.EmployeeUuid).IsUnique();

                //リレーション設定
                entity.HasOne(e => e.Department)
                      .WithMany()
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EmployeeAccountEntity>(entity =>
            {
                //Unique設定
                entity.HasIndex(e => e.AccountUuid).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();

                //リレーション設定
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey(e => e.EmployeeId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // ########### 商品関連 ###########
            modelBuilder.Entity<ProductEntity>(entity =>
            {
                //Unique設定
                entity.HasIndex(e => e.ProductUuid).IsUnique();
                //リレーション設定
                entity.HasOne(p => p.ProductCategory)
                      .WithMany()
                      .HasForeignKey(p => p.ProductCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<ProductCategoryEntity>(entity =>
            {
                // 識別Id(category_uuid)は一意
                entity.HasIndex(e => e.CategoryUuid).IsUnique();
            });

            modelBuilder.Entity<ProductStockEntity>(entity =>
            {
                //Unique設定
                entity.HasIndex(e => e.StockUuid).IsUnique();
                entity.HasIndex(e => e.ProductId).IsUnique();
                //リレーション設定
                entity.HasOne(s => s.Product)
                      .WithMany()
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}