namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 商品削除ユースケースのインターフェイス
/// </summary>
public interface IDeleteProductUsecase
{
    /// <summary>
    /// 指定された商品を削除する
    /// </summary>
    /// <param name="productUuid">削除対象の商品UUID</param>
    Task ExecuteAsync(string productUuid);
}