using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 注文ステータスEntityからドメインモデルへ復元するAdapter。
/// </summary>
public class OrderStatusEntityAdapter
    : IRestorer<OrderStatus, OrderStatusEntity>
{
    /// <summary>
    /// 注文ステータスEntityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="target">注文ステータスEntity。</param>
    /// <returns>注文ステータスのドメインモデル。</returns>
    public Task<OrderStatus> RestoreAsync(OrderStatusEntity target)
    {
        var orderStatus = new OrderStatus(
            target.Id,
            target.Name
        );

        return Task.FromResult(orderStatus);
    }
}