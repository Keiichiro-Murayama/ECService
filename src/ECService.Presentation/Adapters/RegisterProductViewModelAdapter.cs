using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// RegisterProductRequestから
/// Productへ変換するアダプタ
/// </summary>
//石原:変更 ImageUrlを別引数で受け取るためIRestorerの実装を外す
public class RegisterProductViewModelAdapter
{
    /// <summary>
    /// 商品登録リクエストから
    /// 商品ドメインを復元する
    /// </summary>
    /// <param name="target">
    /// 商品登録リクエスト
    /// </param>
    /// <param name="imageUrl">
    /// Azure Blob Storageへ保存した画像のURL
    /// </param>
    /// <returns>
    /// 商品ドメイン
    /// </returns>
    //石原:変更 画像URLはリクエストDTOではなくControllerから受け取る
    public Task<Product> RestoreAsync(
        RegisterProductRequest target,
        string imageUrl)
    {
        ArgumentNullException.ThrowIfNull(
            target);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            imageUrl);

        var category =
            new ProductCategory(
                target.CategoryUuid,
                string.Empty);

        ProductStock productStock =
            ProductStock.Create(
                target.Stock!.Value);

        Product product =
            Product.Create(
                target.ProductName,
                target.Price!.Value,
                imageUrl,
                category,
                productStock);

        return Task.FromResult(product);
    }
}