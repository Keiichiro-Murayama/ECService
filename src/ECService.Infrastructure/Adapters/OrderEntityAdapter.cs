using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 注文Entityからドメインモデルへ復元するAdapter。
/// </summary>
public class OrderEntityAdapter
    : IRestorer<Order, OrdersEntity>
{
    private readonly CustomerEntityAdapter _customerEntityAdapter;
    private readonly OrderStatusEntityAdapter _orderStatusEntityAdapter;
    private readonly OrderDetailEntityAdapter _orderDetailEntityAdapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public OrderEntityAdapter(
        CustomerEntityAdapter customerEntityAdapter,
        OrderStatusEntityAdapter orderStatusEntityAdapter,
        OrderDetailEntityAdapter orderDetailEntityAdapter)
    {
        _customerEntityAdapter = customerEntityAdapter;
        _orderStatusEntityAdapter = orderStatusEntityAdapter;
        _orderDetailEntityAdapter = orderDetailEntityAdapter;
    }

    /// <summary>
    /// 注文Entityから注文ドメインモデルへ復元する。
    /// </summary>
    /// <param name="target">注文Entity。</param>
    /// <returns>注文ドメインモデル。</returns>
    public async Task<Order> RestoreAsync(OrdersEntity target)
    {
        var customer =
            await _customerEntityAdapter.RestoreAsync(target.Customer);

        var orderStatus =
            await _orderStatusEntityAdapter.RestoreAsync(
                target.OrderStatus
            );

        var orderDetails = new List<OrderDetail>();

        foreach (var orderDetailEntity in target.OrdersDetails)
        {
            var orderDetail =
                await _orderDetailEntityAdapter.RestoreAsync(
                    orderDetailEntity
                );

            orderDetails.Add(orderDetail);
        }

        return new Order(
            target.OrderUuid.ToString(),
            target.OrderDate,
            target.AmountTotal,
            customer,
            orderStatus,
            orderDetails
        );
    }
}