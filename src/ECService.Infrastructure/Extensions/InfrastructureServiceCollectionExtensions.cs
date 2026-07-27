using Azure.Storage.Blobs; //石原:追加

using ECService.Application.Usecases.Interfaces; //石原:追加
using ECService.Application.Usecases.UnitOfWorks;

using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Repositories;
using ECService.Infrastructure.Storages; //石原:追加
using ECService.Infrastructure.UnitOfWorks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Infrastructure.Extensions;

/// <summary>
/// インフラストラクチャ層の構成要素を
/// DIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// インフラストラクチャ層の構成要素を登録する。
    /// </summary>
    /// <param name="services">
    /// DIコンテナ。
    /// </param>
    /// <param name="connectionString">
    /// データベースの接続文字列。
    /// </param>
    /// <param name="blobStorageConnectionString">
    /// Azure Blob Storageの接続文字列。
    /// </param>
    /// <param name="productImageContainerName">
    /// 商品画像を保存するコンテナー名。
    /// </param>
    /// <returns>
    /// DIコンテナ。
    /// </returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        //石原:追加
        string blobStorageConnectionString,
        //石原:追加
        string productImageContainerName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            connectionString);

        //石原:追加
        ArgumentException.ThrowIfNullOrWhiteSpace(
            blobStorageConnectionString);

        //石原:追加
        ArgumentException.ThrowIfNullOrWhiteSpace(
            productImageContainerName);

        // DbContext
        services.AddDbContext<AppDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        /*
         * Azure Blob Storage
         *
         * Azure SDKのクライアントは
         * スレッドセーフなのでSingletonで登録する。
         */

        //石原:追加
        services.AddSingleton(
            new BlobContainerClient(
                blobStorageConnectionString,
                productImageContainerName));

        //石原:追加
        services.AddScoped<
            IProductImageStorage,
            AzureBlobProductImageStorage>();

        // Adapter
        services.AddScoped<ProductEntityAdapter>();

        services.AddScoped<ProductStockEntityAdapter>();

        services.AddScoped<
            ProductCategoryEntityAdapter>();

        services.AddScoped<EmployeeEntityAdapter>();

        services.AddScoped<
            EmployeeAccountEntityAdapter>();

        services.AddScoped<CustomerEntityAdapter>();

        services.AddScoped<OrderStatusEntityAdapter>();

        services.AddScoped<OrderDetailEntityAdapter>();

        services.AddScoped<OrderEntityAdapter>();

        // ProductFactory
        services.AddScoped<ProductFactory>();

        // Adapter Interface
        services.AddScoped<
            IRestorer<
                ProductCategory,
                ProductCategoryEntity>,
            ProductCategoryEntityAdapter>();

        services.AddScoped<
            IConverter<
                ProductCategory,
                ProductCategoryEntity>,
            ProductCategoryEntityAdapter>();

        services.AddScoped<
            IRestorer<
                ProductStock,
                ProductStockEntity>,
            ProductStockEntityAdapter>();

        services.AddScoped<
            IRestorer<
                Employee,
                EmployeeEntity>,
            EmployeeEntityAdapter>();

        services.AddScoped<
            IConverter<
                EmployeeAccount,
                EmployeeAccountEntity>,
            EmployeeAccountEntityAdapter>();

        services.AddScoped<
            IRestorer<
                EmployeeAccount,
                EmployeeAccountEntity>,
            EmployeeAccountEntityAdapter>();

        services.AddScoped<
            IRestorer<
                Customer,
                CustomerEntity>,
            CustomerEntityAdapter>();

        services.AddScoped<
            IRestorer<
                OrderStatus,
                OrderStatusEntity>,
            OrderStatusEntityAdapter>();

        services.AddScoped<
            IRestorer<
                OrderDetail,
                OrdersDetailEntity>,
            OrderDetailEntityAdapter>();

        services.AddScoped<
            IRestorer<
                Order,
                OrdersEntity>,
            OrderEntityAdapter>();

        // UnitOfWork
        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();

        // Repository
        services.AddScoped<
            IProductRepository,
            ProductRepository>();

        services.AddScoped<
            IProductCategoryRepository,
            ProductCategoryRepository>();

        services.AddScoped<
            IEmployeeRepository,
            EmployeeRepository>();

        services.AddScoped<
            IEmployeeAccountRepository,
            EmployeeAccountRepository>();

        services.AddScoped<
            IOrderRepository,
            OrderRepository>();

        services.AddScoped<
            IOrderStatusRepository,
            OrderStatusRepository>();

        return services;
    }
}