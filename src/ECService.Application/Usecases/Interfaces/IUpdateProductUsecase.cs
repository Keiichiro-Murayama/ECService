using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 商品修正ユースケースインターフェイス。
/// </summary>
public interface IUpdateProductUsecase
{
    /// <summary>
    /// 商品情報を修正する。
    /// </summary>
    /// <param name="productUuid">商品UUID。</param>
    /// <param name="name">商品名。</param>
    /// <param name="price">価格。</param>
    /// <param name="stock">在庫数。</param>
    /// <param name="categoryUuid">商品カテゴリUUID。</param>
    /// <param name="imageUrl">画像URL。</param>
    /// <returns>修正後の商品。</returns>
    Task<Product> ExecuteAsync(
        string productUuid,
        string name,
        int price,
        int stock,
        string categoryUuid,
        string imageUrl);
}