using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// RegisterProductRequestからドメインオブジェクト:Productへ変換するアダプタ
/// </summary>
public class RegisterProductViewModelAdapter : IRestorer<Product, RegisterProductRequest>
{
    /// <summary>
    /// RegisterProductRequestからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">商品登録リクエスト</param>
    /// <returns>商品ドメイン</returns>
    public Task<Product> RestoreAsync(RegisterProductRequest target)
    {
        var category = new ProductCategory(
            target.CategoryUuid,
            string.Empty);

        var productStock = ProductStock.Create(target.Stock!.Value);

        var product = Product.Create(
            target.ProductName,
            target.Price!.Value,
            target.ImageUrl,
            category,
            productStock);

        return Task.FromResult(product);
    }
}//石原:本来Adapterの仕事外のエラーチェックが含まれていたので変更