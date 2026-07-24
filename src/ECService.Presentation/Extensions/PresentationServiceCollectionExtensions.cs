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
        // 商品詳細取得用ViewModelAdapter
        services.AddScoped<GetProductViewModelAdapter>();

        // 商品検索用ViewModelAdapter
        services.AddScoped<SearchProductsViewModelAdapter>();

        // カテゴリ登録用ViewModelAdapter
        services.AddScoped<RegisterCategoryViewModelAdapter>();

        // 商品登録用ViewModelAdapter
        services.AddScoped<RegisterProductViewModelAdapter>();

        // カテゴリ取得用ViewModelAdapter
        services.AddScoped<GetCategoriesViewModelAdapter>();

        // ログイン用ViewModelAdapter
        services.AddScoped<LoginViewModelAdapter>();

        // 未登録用ViewModelAdapter
        services.AddScoped<UnregisteredEmployeesViewModelAdapter>();
        
        // 購入履歴検索用ViewModelAdapter
        services.AddScoped<SearchOrderHistoriesViewModelAdapter>();

        // 注文ステータス更新用ViewModelAdapter
        services.AddScoped<GetOrderStatusUpdateViewModelAdapter>();

        // 注文ステータス更新結果用ViewModelAdapter
        services.AddScoped<UpdateOrderStatusViewModelAdapter>();
        return services;
    }
}