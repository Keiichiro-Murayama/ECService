using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// Productドメインモデルを商品詳細レスポンスへ変換するAdapter。
/// </summary>
public class GetProductViewModelAdapter
{
    /// <summary>
    /// Productドメインモデルを商品詳細レスポンスへ変換する。
    /// </summary>
    /// <param name="product">商品ドメインモデル。</param>
    /// <returns>商品詳細レスポンス。</returns>
    public GetProductInfoResponse Convert(Product product)
    {
        return new GetProductInfoResponse
        {
            ProductUuid = product.ProductUuid,
            ProductName = product.Name,
            Price = product.Price,
            Stock = product.ProductStock.Quantity,
            CategoryUuid = product.ProductCategory.CategoryUuid,
            ImageUrl = product.ImageUrl
        };
    }
}