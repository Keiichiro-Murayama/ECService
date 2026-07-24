using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 注文ドメインオブジェクトを購入履歴検索用ViewModelへ変換するアダプタ
/// </summary>
public class SearchOrderHistoriesViewModelAdapter
{
    /// <summary>
    /// 注文リストを購入履歴検索結果ViewModelへ変換する
    /// </summary>
    /// <param name="orders">注文ドメインオブジェクトのリスト</param>
    /// <returns>購入履歴検索結果ViewModel</returns>
    public SearchOrderHistoriesResponse Convert(List<Order> orders)
    {
        return new SearchOrderHistoriesResponse
        {
            OrderHistories = orders
                .Select(order => new OrderHistoriesItem
                {
                    OrderUuid = order.OrderUuid,
                    PurchaseDate = order.OrderDate,
                    CustomerAccountName = order.Customer.Username,
                    OrderContent = string.Join(
                        "、",
                        order.OrderDetails.Select(orderDetail =>
                            $"{orderDetail.ProductName} × {orderDetail.Count}"
                        )
                    ),
                    OrderStatus = order.OrderStatus.Name
                })
                .ToList()
        };
    }
}