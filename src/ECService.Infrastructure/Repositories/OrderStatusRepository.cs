using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 注文ステータス情報を操作するRepository。
/// </summary>
public class OrderStatusRepository : IOrderStatusRepository
{
    private readonly AppDbContext _context;
    private readonly OrderStatusEntityAdapter _adapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public OrderStatusRepository(
        AppDbContext context,
        OrderStatusEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// 注文ステータス情報を全件取得する。
    /// </summary>
    /// <returns>注文ステータス情報の一覧。</returns>
    public async Task<List<OrderStatus>> SelectAllAsync()
    {
        var entities = await _context.OrderStatuses
            .AsNoTracking()
            .OrderBy(orderStatus => orderStatus.Id)
            .ToListAsync();

        var orderStatuses = new List<OrderStatus>();

        foreach (var entity in entities)
        {
            var orderStatus = await _adapter.RestoreAsync(entity);
            orderStatuses.Add(orderStatus);
        }

        return orderStatuses;
    }

    /// <summary>
    /// 注文ステータスIDに一致する注文ステータスを取得する。
    /// </summary>
    /// <param name="orderStatusId">注文ステータスID。</param>
    /// <returns>
    /// 一致する注文ステータス。存在しない場合はnull。
    /// </returns>
    //石原:追加
    public async Task<OrderStatus?> SelectByIdAsync(
        int orderStatusId)
    {
        var entity = await _context.OrderStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(orderStatus =>
                orderStatus.Id == orderStatusId);

        if (entity is null)
        {
            return null;
        }

        return await _adapter.RestoreAsync(entity);
    }
}