using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 注文ステータス更新画面の表示情報を取得するユースケースインターフェース
/// </summary>
public interface IGetOrderStatusUpdateUsecase
{
    /// <summary>
    /// 対象の注文情報と注文ステータス一覧を取得する
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>
    /// 対象の注文情報と選択可能な注文ステータス一覧
    /// </returns>
    Task<(
        Order Order,
        List<OrderStatus> OrderStatuses
    )> ExecuteAsync(string orderUuid);
}