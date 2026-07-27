using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 更新後の注文情報を注文ステータス更新結果ViewModelへ変換するアダプタ。
/// </summary>
public class UpdateOrderStatusViewModelAdapter
{
    /// <summary>
    /// 更新後の注文情報と更新日時をViewModelへ変換する。
    /// </summary>
    /// <param name="order">更新後の注文情報。</param>
    /// <param name="updatedAt">更新処理完了日時。</param>
    /// <returns>注文ステータス更新結果。</returns>
    public UpdateOrderStatusResponse Convert(
        Order order,
        DateTime updatedAt)
    {
        return new UpdateOrderStatusResponse
        {
            OrderUuid = order.OrderUuid,
            OrderStatusId = order.OrderStatus.Id,
            OrderStatusName = order.OrderStatus.Name,
            UpdatedAt = updatedAt
        };
    }
}