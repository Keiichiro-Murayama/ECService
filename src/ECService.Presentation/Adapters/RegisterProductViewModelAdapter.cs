using ECService.Domain.Adapters;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 商品登録リクエストから商品ドメインへ変換するアダプタ
/// </summary>
public class RegisterProductViewModelAdapter :
    IRestorer<Product, RegisterProductRequest>
{
    /// <summary>
    /// 商品登録リクエストから商品ドメインを復元する
    /// </summary>
    public Task<Product> RestoreAsync(RegisterProductRequest target)
    {
        if (target.Price is null ||
            target.Stock is null ||
            string.IsNullOrWhiteSpace(target.CategoryUuid) ||
            !Guid.TryParse(target.CategoryUuid, out _))
        {
            throw new DomainException("入力値に不備があります。");
        }

        var productCategory = new ProductCategory(
            target.CategoryUuid,
            string.Empty);

        var productStock = ProductStock.Create(target.Stock.Value);

        var product = Product.Create(
            target.ProductName,
            target.Price.Value,
            target.ImageUrl,
            productCategory,
            productStock);

        return Task.FromResult(product);
    }
}