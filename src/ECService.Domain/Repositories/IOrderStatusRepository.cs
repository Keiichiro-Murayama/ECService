using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 注文ステータス情報を操作するRepositoryのインターフェース。
/// </summary>
public interface IOrderStatusRepository
{
    /// <summary>
    /// 注文ステータス情報を全件取得する。
    /// </summary>
    /// <returns>注文ステータス情報の一覧。</returns>
    Task<List<OrderStatus>> SelectAllAsync();

    /// <summary>
    /// 注文ステータスIDに一致する注文ステータスを取得する。
    /// </summary>
    /// <param name="orderStatusId">注文ステータスID。</param>
    /// <returns>
    /// 一致する注文ステータス。存在しない場合はnull。
    /// </returns>
    //石原:追加
    Task<OrderStatus?> SelectByIdAsync(int orderStatusId);
}