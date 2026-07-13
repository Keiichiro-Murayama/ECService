using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.Interfaces;
using ECService.Application.Authentications;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Application.Extensions;

/// <summary>
/// アプリケーション層の構成要素をDIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// アプリケーション層の構成要素を登録する。
    /// </summary>
    /// <param name="services">DIコンテナ。</param>
    /// <returns>DIコンテナ。</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGetProductInfoUsecase, GetProductInfoUsecase>();
        services.AddScoped<IUpdateProductUsecase, UpdateProductUsecase>();
        services.AddScoped<IDeleteProductUsecase, DeleteProductUsecase>();
        services.AddScoped<ISearchProductsUsecase, SearchProductsUsecase>();
        services.AddScoped<IGetUnregisteredEmployeesUsecase, GetUnregisteredEmployeesUsecase>();
        services.AddScoped<IRegisterEmployeeAccountUsecase, RegisterEmployeeAccountUsecase>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IGetCategoriesUsecase, GetCategoriesUsecase>();
        services.AddScoped<IRegisterProductCategoryUsecase, RegisterProductCategoryUsecase>();
        services.AddScoped<IRegisterProductUsecase, RegisterProductUsecase>();
        services.AddScoped<ILoginUsecase, LoginUsecase>();
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();

        return services;
    }
}