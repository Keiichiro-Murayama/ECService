using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 注文ステータスを更新するユースケース。
/// </summary>
public class UpdateOrderStatusUsecase
    : IUpdateOrderStatusUsecase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public UpdateOrderStatusUsecase(
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository)
    {
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
    }

    /// <summary>
    /// 指定された注文のステータスを更新する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <param name="orderStatusId">
    /// 変更後の注文ステータスID。
    /// </param>
    /// <returns>
    /// 更新後の注文情報と更新処理完了日時。
    /// </returns>
    public async Task<(
        Order Order,
        DateTime UpdatedAt
    )> ExecuteAsync(
        string orderUuid,
        int orderStatusId)
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

        if (orderStatusId <= 0)
        {
            throw new DomainException(
                "注文ステータスIDは1以上で指定してください。",
                nameof(orderStatusId));
        }

        var order =
            await _orderRepository.SelectByOrderUuidAsync(
                orderUuid
            );

        if (order is null)
        {
            throw new DomainException(
                "指定された注文情報が見つかりません。",
                nameof(orderUuid));
        }

        var newOrderStatus =
            await _orderStatusRepository.SelectByIdAsync(
                orderStatusId
            );

        if (newOrderStatus is null)
        {
            throw new DomainException(
                "指定された注文ステータスが見つかりません。",
                nameof(orderStatusId));
        }

        order.ChangeOrderStatus(newOrderStatus);

        await _orderRepository.UpdateOrderStatusAsync(order);

        var updatedAt = DateTime.UtcNow;

        return (
            order,
            updatedAt
        );
    }
}