using ECService.Presentation.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Presentation.Extensions;

/// <summary>
/// Presentation層の依存関係を登録する拡張クラス
/// </summary>
public static class PresentationServiceCollectionExtensions
{
    /// <summary>
    /// Presentation層の依存関係をDIコンテナに登録する
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <returns>サービスコレクション</returns>
    public static IServiceCollection AddPresentationLayerDependencies(
        this IServiceCollection services)
    {
        // 商品検索結果をViewModelへ変換するアダプタ
        services.AddScoped<SearchProductsViewModelAdapter>();

        return services;
    }
}