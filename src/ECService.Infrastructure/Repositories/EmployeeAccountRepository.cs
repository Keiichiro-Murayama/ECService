using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Data;
using ECService.Infrastructure.Adapters;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 担当者アカウント情報を操作するRepository。
/// </summary>
public class EmployeeAccountRepository : IEmployeeAccountRepository
{
    private readonly AppDbContext _context;
    private readonly EmployeeAccountEntityAdapter _adapter;

    public EmployeeAccountRepository(
        AppDbContext context,
        EmployeeAccountEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// アカウント名に一致する担当者アカウントを取得する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>担当者アカウント。存在しない場合はnull。</returns>
    public async Task<EmployeeAccount?> SelectByAccountNameAsync(string accountName)
    {
        var entity = await _context.EmployeeAccounts
            .Include(employeeAccount => employeeAccount.Employee)
            .ThenInclude(employee => employee.Department)
            .FirstOrDefaultAsync(employeeAccount =>
                employeeAccount.Name == accountName);

        if (entity is null)
        {
            return null;
        }

        return _adapter.ToDomain(entity);
    }

    /// <summary>
    /// 指定したアカウント名が既に存在するか確認する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>存在する場合true。</returns>
    public async Task<bool> ExistsByAccountNameAsync(string accountName)
    {
        return await _context.EmployeeAccounts
            .AnyAsync(employeeAccount =>
                employeeAccount.Name == accountName);
    }

    /// <summary>
    /// 担当者アカウントを登録する。
    /// </summary>
    /// <param name="employeeAccount">担当者アカウント。</param>
    public async Task CreateAsync(EmployeeAccount employeeAccount)
    {
        var entity = _adapter.ToEntity(employeeAccount);

        await _context.EmployeeAccounts.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}