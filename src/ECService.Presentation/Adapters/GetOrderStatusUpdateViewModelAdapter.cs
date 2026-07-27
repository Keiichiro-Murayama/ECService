using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 注文情報と注文ステータス一覧を
/// 注文ステータス更新画面用ViewModelへ変換するアダプタ。
/// </summary>
public class GetOrderStatusUpdateViewModelAdapter
{
    /// <summary>
    /// 注文情報と注文ステータス一覧をViewModelへ変換する。
    /// </summary>
    /// <param name="order">注文情報。</param>
    /// <param name="orderStatuses">注文ステータス一覧。</param>
    /// <returns>注文ステータス更新画面の表示情報。</returns>
    public GetOrderStatusUpdateResponse Convert(
        Order order,
        List<OrderStatus> orderStatuses)
    {
        return new GetOrderStatusUpdateResponse
        {
            OrderUuid = order.OrderUuid,
            OrderDate = order.OrderDate,
            CustomerAccountName = order.Customer.Username,

            OrderContent = string.Join(
                "、",
                order.OrderDetails.Select(orderDetail =>
                    $"{orderDetail.ProductName} × {orderDetail.Count}"
                )
            ),

            CurrentOrderStatusId = order.OrderStatus.Id,
            CurrentOrderStatusName = order.OrderStatus.Name,

            OrderStatuses = orderStatuses
                .Select(orderStatus => new OrderStatusItem
                {
                    OrderStatusId = orderStatus.Id,
                    OrderStatusName = orderStatus.Name
                })
                .ToList()
        };
    }
}