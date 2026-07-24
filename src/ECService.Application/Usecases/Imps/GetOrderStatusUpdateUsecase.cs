using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 注文ステータス更新画面の表示情報を取得するユースケース。
/// </summary>
public class GetOrderStatusUpdateUsecase
    : IGetOrderStatusUpdateUsecase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public GetOrderStatusUpdateUsecase(
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
    }

    /// <summary>
    /// 対象の注文情報と注文ステータス一覧を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <returns>
    /// 対象の注文情報と注文ステータス一覧。
    /// </returns>
    public async Task<(
        Order Order,
        List<OrderStatus> OrderStatuses
    )> ExecuteAsync(string orderUuid)
    {
        if (string.IsNullOrWhiteSpace(orderUuid))
        {
            throw new DomainException(
                "注文UUIDは必須です。",
                nameof(orderUuid));
        }

        if (!Guid.TryParse(orderUuid, out _))
        {
            throw new DomainException(
                "注文UUIDの形式が不正です。",
                nameof(orderUuid));
        }

        var order =
            await _orderRepository.SelectByOrderUuidAsync(orderUuid);

        if (order is null)
        {
            throw new DomainException(
                "指定された注文情報が見つかりません。",
                nameof(orderUuid));
        }

        var orderStatuses =
            await _orderStatusRepository.SelectAllAsync();

        return (
            order,
            orderStatuses
        );
    }
}