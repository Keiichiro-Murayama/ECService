using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 商品検索ユースケースのインターフェイス
/// </summary>
public interface ISearchProductsUseCase
{
    /// <summary>
    /// 商品を検索する
    /// カテゴリUUIDが未指定の場合は全商品を取得する
    /// カテゴリUUIDが指定されている場合はカテゴリ別の商品を取得する
    /// </summary>
    /// <param name="categoryUuid">検索対象の商品カテゴリUUID</param>
    /// <returns>検索結果の商品一覧</returns>
    Task<List<Product>> ExecuteAsync(string? categoryUuid);
}