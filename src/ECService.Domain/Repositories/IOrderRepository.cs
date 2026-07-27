using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 注文情報を操作するRepositoryのインターフェース
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 指定された検索条件に一致する購入履歴を取得する
    /// </summary>
    /// <param name="purchaseDate">
    /// 購入日。nullの場合は購入日による絞り込みを行わない
    /// </param>
    /// <param name="customerAccountName">
    /// 顧客アカウント名。nullまたは空文字の場合は
    /// 顧客アカウント名による絞り込みを行わない
    /// </param>
    /// <returns>検索条件に一致する購入履歴一覧</returns>
    Task<List<Order>> SelectBySearchConditionsAsync(
        DateOnly? purchaseDate,
        string? customerAccountName);

    /// <summary>
    /// 注文UUIDに一致する注文情報を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <returns>
    /// 一致する注文情報。存在しない場合はnull。
    /// </returns>
    //石原:追加
    Task<Order?> SelectByOrderUuidAsync(string orderUuid);

    /// <summary>
    /// 注文ステータスの変更を保存する。
    /// </summary>
    /// <param name="order">更新対象の注文。</param>
    //石原:追加
    Task UpdateOrderStatusAsync(Order order);
}