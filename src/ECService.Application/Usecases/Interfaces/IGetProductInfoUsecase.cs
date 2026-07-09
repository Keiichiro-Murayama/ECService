using ECService.Domain.Models;

namespace ECService.Applications.Usecases.Interfaces;

/// <summary>
/// 商品詳細取得ユースケースインターフェイス。
/// </summary>
public interface IGetProductInfoUsecase
{
    /// <summary>
    /// 商品UUIDに一致する商品情報を取得する。
    /// </summary>
    /// <param name="productUuid">商品UUID。</param>
    /// <returns>商品情報。存在しない場合はnull。</returns>
    Task<Product?> ExecuteAsync(string productUuid);
}