using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 商品検索ユースケースインターフェイス
/// </summary>
public interface ISearchProductsUsecase
{
    /// <summary>
    /// 商品検索を実行する
    /// </summary>
    /// <param name="categoryUuid">
    /// 商品カテゴリUUID。
    /// 指定されない場合は全商品を検索する。
    /// </param>
    /// <returns>商品一覧</returns>
    Task<List<Product>> ExecuteAsync(string? categoryUuid);
}