using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.Interfaces;
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

        return services;
    }
}