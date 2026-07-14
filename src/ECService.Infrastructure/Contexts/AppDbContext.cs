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
        public DbSet<CustomerEntity> CustomerEntities => Set<CustomerEntity>();
        public DbSet<OrderStatusEntity> OrderStatuses => Set<OrderStatusEntity>();
        public DbSet<OrdersEntity> Orders => Set<OrdersEntity>();
        public DbSet<OrdersDetailEntity> OrdersDetails => Set<OrdersDetailEntity>();
        public DbSet<PaymentMethodEntity> PaymentMethods => Set<PaymentMethodEntity>();

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
                // entity.HasOne(s => s.Product)
                //       .WithMany()
                //       .HasForeignKey(s => s.ProductId)
                //       .OnDelete(DeleteBehavior.Restrict);

                //石原:商品との1対1リレーション設定に変更、変なら上記に戻す
                entity.HasOne(s => s.Product)
                      .WithOne(p => p.ProductStock)
                      .HasForeignKey<ProductStockEntity>(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });


            // ########### 注文関連 ###########
            //注文
            modelBuilder.Entity<OrdersEntity>(entity =>
            {
                entity.HasIndex(e => e.OrderUuid).IsUnique();

                entity.HasOne(e => e.OrderStatus)
                      .WithMany()
                      .HasForeignKey(e => e.OrderStatusId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.PaymentMethod)
                      .WithMany()
                      .HasForeignKey(e => e.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(o => o.OrdersDetails)
                      .WithOne(d => d.Order)
                      .HasForeignKey(d => d.OrderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //注文明細
            modelBuilder.Entity<OrdersDetailEntity>(entity =>
            {
                entity.HasOne(e => e.Product)
                      .WithMany()
                      .HasForeignKey(e => e.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            //注文ステータス
            modelBuilder.Entity<OrderStatusEntity>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();

            });

            //支払方法
            modelBuilder.Entity<PaymentMethodEntity>(entity =>
            {
                entity.HasIndex(p => p.Name).IsUnique();
            });

            // ########### 顧客関連 ###########
            modelBuilder.Entity<CustomerEntity>(entity =>
            {
                entity.HasIndex(e => e.CustomerUuid).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.MailAddress).IsUnique();

                entity.HasMany(e => e.OrdersEntities)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}