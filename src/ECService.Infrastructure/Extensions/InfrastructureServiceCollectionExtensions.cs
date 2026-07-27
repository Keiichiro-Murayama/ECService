using Azure.Storage.Blobs;

using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;

using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Repositories;
using ECService.Infrastructure.Storages;
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
    /// <param name="containerSasUrl">
    /// 商品画像用BlobコンテナーのSAS URL。
    /// </param>
    /// <returns>
    /// DIコンテナ。
    /// </returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string containerSasUrl) //石原:変更 Blobの接続文字列とコンテナー名ではなくSAS URLを受け取る
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            connectionString);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            containerSasUrl); //石原:変更 コンテナーSAS URLの未設定を確認する

        if (
            !Uri.TryCreate(
                containerSasUrl,
                UriKind.Absolute,
                out Uri? containerSasUri))
        {
            throw new ArgumentException(
                "Azure Blob StorageのコンテナーSAS URLの形式が正しくありません。",
                nameof(containerSasUrl));
        }

        if (
            !string.Equals(
                containerSasUri.Scheme,
                Uri.UriSchemeHttps,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Azure Blob StorageのコンテナーSAS URLはHTTPSで指定してください。",
                nameof(containerSasUrl));
        }

        if (string.IsNullOrWhiteSpace(
                containerSasUri.Query))
        {
            throw new ArgumentException(
                "Azure Blob StorageのコンテナーSAS URLにSASトークンが含まれていません。",
                nameof(containerSasUrl));
        }

        // DbContext
        services.AddDbContext<AppDbContext>(
            options =>
                options.UseNpgsql(
                    connectionString));

        /*
         * Azure Blob Storage
         *
         * SASトークンを含むコンテナーURLから
         * BlobContainerClientを生成する。
         */

        services.AddSingleton(
            new BlobContainerClient(
                containerSasUri)); //石原:変更 コンテナーSAS URLを使用してBlobへ接続する

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