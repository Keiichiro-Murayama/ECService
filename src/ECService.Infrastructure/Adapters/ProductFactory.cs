using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Exceptions;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 商品集約を復元するFactory
/// </summary>
public class ProductFactory
{
    private readonly IRestorer<ProductCategory, ProductCategoryEntity> _categoryRestorer;
    private readonly IRestorer<ProductStock, ProductStockEntity> _productStockRestorer;

    public ProductFactory(
        IRestorer<ProductCategory, ProductCategoryEntity> categoryRestorer,
        IRestorer<ProductStock, ProductStockEntity> productStockRestorer)
    {
        _categoryRestorer = categoryRestorer;
        _productStockRestorer = productStockRestorer;
    }

    /// <summary>
    /// ProductEntity、ProductCategoryEntity、ProductStockEntityからProductを復元する
    /// </summary>
    public async Task<Product> Factory(
        ProductEntity productEntity,
        ProductStockEntity productStockEntity,
        ProductCategoryEntity productCategoryEntity)
    {
        // nullチェック
        _ = productEntity ?? throw new InternalException("引数productEntityがnullです。");
        _ = productStockEntity ?? throw new InternalException("引数productStockEntityがnullです。");
        _ = productCategoryEntity ?? throw new InternalException("引数productCategoryEntityがnullです。");

        // カテゴリEntity → カテゴリDomain
        var productCategory =
            await _categoryRestorer.RestoreAsync(productCategoryEntity);

        // 在庫Entity → 在庫Domain
        var productStock =
            await _productStockRestorer.RestoreAsync(productStockEntity);

        // 商品Domainを復元
        return Product.Restore(
            productEntity.ProductUuid.ToString(),
            productEntity.Name,
            productEntity.Price,
            productEntity.ImageUrl ?? "",
            productCategory,
            productEntity.DeleteFlag,

            productStock
        );
    }
}