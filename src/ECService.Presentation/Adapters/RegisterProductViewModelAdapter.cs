using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
namespace ECService.Presentation.Adapters;
/// <summary>
/// RegisterProductViewModelからドメインオブジェクト:Productへ変換するアダプタ
/// </summary>
public class RegisterProductViewModelAdapter : IRestorer<Product, RegisterProductRequest>
{
    /// <summary>
    /// RegisterProductViewModelからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">ユースケース:[新商品を登録する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<Product> RestoreAsync(RegisterProductRequest target)
    {
        // 商品カテゴリを生成する
        var category = new ProductCategory(target.CategoryUuid, target.ProductName);
        // 商品在庫を生成する
        var productStock = ProductStock.Create(target.Stock);
        // 商品を生成する
        var product = Product.Create(
                                        target.ProductName,
                                        target.Price,
                                        target.ImageUrl,
                                        category,
                                        productStock
                                    );
        // 商品カテゴリと商品在庫を設定する
        product.ChangeCategory(category);
        product.ChangeStock(productStock);
        return Task.FromResult(product);
    }
}