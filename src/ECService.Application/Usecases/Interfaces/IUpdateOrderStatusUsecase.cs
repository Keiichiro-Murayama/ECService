using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 注文ステータスを更新するユースケースインターフェース。
/// </summary>
public interface IUpdateOrderStatusUsecase
{
    /// <summary>
    /// 指定された注文のステータスを更新する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <param name="orderStatusId">変更後の注文ステータスID。</param>
    /// <returns>
    /// 更新後の注文情報と更新処理完了日時。
    /// </returns>
    Task<(
        Order Order,
        DateTime UpdatedAt
    )> ExecuteAsync(
        string orderUuid,
        int orderStatusId);
}