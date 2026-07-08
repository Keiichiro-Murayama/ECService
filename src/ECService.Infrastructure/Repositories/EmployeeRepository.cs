using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Data;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 社員情報を操作するRepository。
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;
    private readonly EmployeeEntityAdapter _adapter;

    public EmployeeRepository(
        AppDbContext context,
        EmployeeEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// 担当者アカウントが未登録の社員情報を取得する。
    /// </summary>
    /// <returns>未登録社員情報の一覧。</returns>
    public async Task<List<Employee>> SelectUnregisteredAsync()
    {
        var entities = await _context.Employees
            .Where(employee =>
                !_context.EmployeeAccounts
                    .Any(employeeAccount =>
                        employeeAccount.EmployeeId == employee.Id))
            .ToListAsync();

        return entities
            .Select(entity => _adapter.Restore(entity))
            .ToList();
    }

    /// <summary>
    /// 社員UUIDに一致する社員情報を取得する。
    /// </summary>
    /// <param name="employeeUuid">社員UUID。</param>
    /// <returns>社員情報。存在しない場合はnull。</returns>
    public async Task<Employee?> SelectByUuidAsync(string employeeUuid)
    {
        var entity = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmployeeUuid == employeeUuid);

        if (entity is null)
        {
            return null;
        }

        return _adapter.Restore(entity);
    }
}