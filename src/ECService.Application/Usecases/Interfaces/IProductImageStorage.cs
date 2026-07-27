namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 商品画像の保存処理を提供するインターフェース
/// </summary>
public interface IProductImageStorage
{
    /// <summary>
    /// 商品画像を保存する
    /// </summary>
    /// <param name="imageStream">
    /// 画像ファイルのストリーム
    /// </param>
    /// <param name="productName">
    /// Blobの保存名に使用する商品名
    /// </param>
    /// <param name="contentType">
    /// 画像のContent-Type
    /// </param>
    /// <param name="cancellationToken">
    /// キャンセルトークン
    /// </param>
    /// <returns>
    /// 保存した画像の公開URL
    /// </returns>
    Task<string> UploadAsync(
        Stream imageStream,
        string productName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存済みの商品画像を削除する
    /// </summary>
    /// <param name="imageUrl">
    /// 削除する画像のURL
    /// </param>
    /// <param name="cancellationToken">
    /// キャンセルトークン
    /// </param>
    Task DeleteAsync(
        string imageUrl,
        CancellationToken cancellationToken = default);
}