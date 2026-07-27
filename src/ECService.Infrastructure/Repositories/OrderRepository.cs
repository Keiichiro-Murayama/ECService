using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 注文Repository。
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly OrderEntityAdapter _orderEntityAdapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public OrderRepository(
        AppDbContext context,
        OrderEntityAdapter orderEntityAdapter)
    {
        _context = context;
        _orderEntityAdapter = orderEntityAdapter;
    }

    /// <summary>
    /// 指定された検索条件に一致する購入履歴を取得する。
    /// </summary>
    /// <param name="purchaseDate">
    /// 購入日。nullの場合は購入日による絞り込みを行わない。
    /// </param>
    /// <param name="customerAccountName">
    /// 顧客アカウント名。nullまたは空文字の場合は
    /// 顧客アカウント名による絞り込みを行わない。
    /// </param>
    /// <returns>検索条件に一致する購入履歴一覧。</returns>
    public async Task<List<Order>> SelectBySearchConditionsAsync(
        DateOnly? purchaseDate,
        string? customerAccountName)
    {
        try
        {
            var query = _context.Orders
                .AsNoTracking()
                .Include(order => order.Customer)
                .Include(order => order.OrderStatus)
                .Include(order => order.OrdersDetails)
                    .ThenInclude(detail => detail.Product)
                    .ThenInclude(product => product.ProductCategory)
                .Include(order => order.OrdersDetails)
                    .ThenInclude(detail => detail.Product)
                    .ThenInclude(product => product.ProductStock)
                .AsQueryable();

            if (purchaseDate.HasValue)
            {
                //石原:変更
                var japanTimeZone =
                    TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");

                var localStartDateTime = DateTime.SpecifyKind(
                    purchaseDate.Value.ToDateTime(TimeOnly.MinValue),
                    DateTimeKind.Unspecified
                );

                var localEndDateTime = localStartDateTime.AddDays(1);

                var startUtc = TimeZoneInfo.ConvertTimeToUtc(
                    localStartDateTime,
                    japanTimeZone
                );

                var endUtc = TimeZoneInfo.ConvertTimeToUtc(
                    localEndDateTime,
                    japanTimeZone
                );

                query = query.Where(order =>
                    order.OrderDate >= startUtc &&
                    order.OrderDate < endUtc);
            }

            if (!string.IsNullOrWhiteSpace(customerAccountName))
            {
                var normalizedCustomerAccountName =
                    customerAccountName.Trim();

                //石原:変更
                query = query.Where(order =>
                    order.Customer.Username ==
                    normalizedCustomerAccountName);
            }

            var orderEntities = await query
                .OrderByDescending(order => order.OrderDate)
                .ToListAsync();

            var orders = new List<Order>();

            foreach (var orderEntity in orderEntities)
            {
                var order =
                    await _orderEntityAdapter.RestoreAsync(orderEntity);

                orders.Add(order);
            }

            return orders;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "購入履歴検索中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 注文UUIDに一致する注文情報を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <returns>
    /// 一致する注文情報。存在しない場合はnull。
    /// </returns>
    //石原:追加
    public async Task<Order?> SelectByOrderUuidAsync(
        string orderUuid)
    {
        if (!Guid.TryParse(orderUuid, out var parsedOrderUuid))
        {
            return null;
        }

        try
        {
            var orderEntity = await _context.Orders
                .AsNoTracking()
                .Include(order => order.Customer)
                .Include(order => order.OrderStatus)
                .Include(order => order.OrdersDetails)
                    .ThenInclude(detail => detail.Product)
                .FirstOrDefaultAsync(order =>
                    order.OrderUuid == parsedOrderUuid);

            if (orderEntity is null)
            {
                return null;
            }

            return await _orderEntityAdapter.RestoreAsync(orderEntity);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "注文情報取得中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 注文ステータスの変更を保存する。
    /// </summary>
    /// <param name="order">更新対象の注文。</param>
    //石原:追加
    public async Task UpdateOrderStatusAsync(Order order)
    {
        try
        {
            if (!Guid.TryParse(
                order.OrderUuid,
                out var parsedOrderUuid))
            {
                throw new InternalException(
                    "注文UUIDの形式が不正です。");
            }

            var orderEntity = await _context.Orders
                .SingleOrDefaultAsync(target =>
                    target.OrderUuid == parsedOrderUuid);

            if (orderEntity is null)
            {
                throw new InternalException(
                    "更新対象の注文情報が存在しません。");
            }

            //石原:追加
            orderEntity.OrderStatusId =
                order.OrderStatus.Id;

            await _context.SaveChangesAsync();
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"注文UUID:{order.OrderUuid}のステータス更新中に予期しないエラーが発生しました。",
                ex);
        }
    }
}