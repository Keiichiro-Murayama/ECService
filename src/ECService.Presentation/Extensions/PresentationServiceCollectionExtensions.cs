using ECService.Presentation.Adapters;
using Microsoft.Extensions.DependencyInjection;

namespace ECService.Presentation.Extensions;

/// <summary>
/// プレゼンテーション層の構成要素をDIコンテナへ登録する拡張メソッドを提供する。
/// </summary>
public static class PresentationServiceCollectionExtensions
{
    /// <summary>
    /// プレゼンテーション層の構成要素を登録する。
    /// </summary>
    /// <param name="services">DIコンテナ。</param>
    /// <returns>DIコンテナ。</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddScoped<GetProductViewModelAdapter>();

        return services;
    }
}