using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 注文明細Entityからドメインモデルへ復元するAdapter。
/// </summary>
public class OrderDetailEntityAdapter
    : IRestorer<OrderDetail, OrdersDetailEntity>
{
    /// <summary>
    /// 注文明細Entityから注文明細ドメインモデルへ復元する。
    /// </summary>
    public Task<OrderDetail> RestoreAsync(
        OrdersDetailEntity target)
    {
        //石原:変更
        var orderDetail = OrderDetail.Restore(
            target.Id,
            target.Product.Name,
            target.Count
        );

        return Task.FromResult(orderDetail);
    }
}