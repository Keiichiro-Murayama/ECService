using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Exceptions;


namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 商品在庫のドメインモデルとEntityを変換するAdapter。
/// </summary>
public class ProductStockEntityAdapter
    : IConverter<ProductStock, ProductStockEntity>, IRestorer<ProductStock, ProductStockEntity>
{
    /// <summary>
    /// 商品在庫のドメインモデルをEntityへ変換する。
    /// </summary>
    public Task<ProductStockEntity> ConvertAsync(ProductStock domain)
    {
        var entity = new ProductStockEntity();
        entity.StockUuid = Guid.Parse(domain.StockUuid);
        entity.Quantity = domain.Quantity;
        return Task.FromResult(entity);

    }

    /// <summary>
    /// 商品在庫Entityからドメインモデルへ復元する。
    /// </summary>
    public Task<ProductStock> RestoreAsync(ProductStockEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        var domain = ProductStock.Restore(target.StockUuid.ToString(),target.Quantity);
        return Task.FromResult(domain);
    }
}