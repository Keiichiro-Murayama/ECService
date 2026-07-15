using ECService.Domain.Adapters;
using ECService.Domain.Exceptions;
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
    /// <returns>商品ドメイン</returns>
    public Task<Product> RestoreAsync(RegisterProductRequest target)
    {
        if (target.Price is null ||
            target.Stock is null ||
            string.IsNullOrWhiteSpace(target.CategoryUuid) ||
            !Guid.TryParse(target.CategoryUuid, out _))
        {
            throw new DomainException(
                "入力値に不備があります。",
                nameof(target.CategoryUuid));
        }

        var category = new ProductCategory(
            target.CategoryUuid,
            string.Empty);

        var productStock = ProductStock.Create(target.Stock.Value);

        var product = Product.Create(
            target.ProductName,
            target.Price.Value,
            target.ImageUrl,
            category,
            productStock);

        return Task.FromResult(product);
    }
}