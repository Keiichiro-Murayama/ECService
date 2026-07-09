using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// ProductEntity から Product ドメインオブジェクトへ変換するFactory
/// </summary>
public class ProductFactory
{
    /// <summary>
    /// EntityからDomainへ復元する
    /// </summary>
    public Product Restore(ProductEntity productEntity, ProductStockEntity stockEntity)
    {
        // 商品カテゴリを復元する
        var category = ProductCategory.Restore(
            productEntity.ProductCategory.CategoryUuid.ToString(),
            productEntity.ProductCategory.Name
        );

        // 商品在庫を復元する
        var stock = ProductStock.Restore(
            stockEntity.StockUuid.ToString(),
            stockEntity.Quantity
        );

        // 商品を復元する
        return Product.Restore(
            productEntity.ProductUuid.ToString(),
            productEntity.Name,
            productEntity.Price,
            productEntity.ImageUrl ?? "",
            productEntity.DeleteFlag,
            category,
            stock
        );
    }
}