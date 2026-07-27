using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 購入履歴を検索するユースケース
/// </summary>
public class SearchOrderHistoriesUsecase : ISearchOrderHistoriesUsecase
{
    private readonly IOrderRepository _orderRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SearchOrderHistoriesUsecase(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// 指定された条件で購入履歴を検索する
    /// </summary>
    public async Task<List<Order>> ExecuteAsync(
        DateOnly? purchaseDate,
        string? customerAccountName)
    {
        var normalizedCustomerAccountName =
            string.IsNullOrWhiteSpace(customerAccountName)
                ? null
                : customerAccountName.Trim();

        return await _orderRepository
            .SelectBySearchConditionsAsync(
                purchaseDate,
                normalizedCustomerAccountName);
    }
}