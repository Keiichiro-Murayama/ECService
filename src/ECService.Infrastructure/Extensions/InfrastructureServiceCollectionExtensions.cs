using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Infrastructure.Extensions;

/// <summary>
/// インフラストラクチャ層の構成要素をDIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class InfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// インフラストラクチャ層の構成要素を登録する。
    /// </summary>
    /// <param name="services">DIコンテナ。</param>
    /// <param name="connectionString">接続文字列。</param>
    /// <returns>DIコンテナ。</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Adapter
        services.AddScoped<ProductEntityAdapter>();
        services.AddScoped<ProductStockEntityAdapter>();
        services.AddScoped<ProductCategoryEntityAdapter>();
        services.AddScoped<EmployeeEntityAdapter>();
        services.AddScoped<EmployeeAccountEntityAdapter>();
        services.AddScoped<ProductFactory>();

        // Repository
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IEmployeeAccountRepository, EmployeeAccountRepository>();

        return services;
    }
}