using ECService.Application.UseCases.UnitOfWorks;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.UnitOfWorks;

/// <summary>
/// IUnitOfWork の実装。
/// EF Core の AppDbContext を用いてトランザクションを制御する。
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="context">DbContext継承クラス。</param>
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// トランザクションを開始する。
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// トランザクションをコミットする。
    /// </summary>
    public async Task CommitAsync()
    {
        await _context.Database.CommitTransactionAsync();
    }

    /// <summary>
    /// トランザクションをロールバックする。
    /// </summary>
    public async Task RollbackAsync()
    {
        await _context.Database.RollbackTransactionAsync();
    }
}