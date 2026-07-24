using ECService.Domain.Models;

namespace ECService.Application.Usecases.Interfaces;

/// <summary>
/// 購入履歴検索ユースケースインターフェイス
/// </summary>
public interface ISearchOrderHistoriesUsecase
{
    /// <summary>
    /// 購入履歴を検索する
    /// </summary>
    /// <param name="purchaseDate">
    /// 購入日。指定されない場合は購入日による絞り込みを行わない。
    /// </param>
    /// <param name="customerAccountName">
    /// 顧客アカウント名。指定されない場合はアカウント名による絞り込みを行わない。
    /// </param>
    /// <returns>購入履歴一覧</returns>
    Task<List<Order>> ExecuteAsync(
        DateOnly? purchaseDate,
        string? customerAccountName);
}