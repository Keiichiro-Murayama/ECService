using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.Imps;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Application.Extensions;

/// <summary>
/// Application層の依存関係を登録する拡張クラス
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    /// <summary>
    /// Application層の依存関係をDIコンテナに登録する
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <returns>サービスコレクション</returns>
    public static IServiceCollection AddApplicationLayerDependencies(
        this IServiceCollection services)
    {
        // ユースケース：[商品を検索する]を実現するインターフェイス
        services.AddScoped<ISearchProductsUseCase, SearchProductsUseCase>();

        return services;
    }
}