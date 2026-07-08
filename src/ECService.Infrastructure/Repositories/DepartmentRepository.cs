using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Data;
using ECService.Infrastructure.Adapters;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 部署情報を操作するRepository。
/// </summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;
    private readonly DepartmentEntityAdapter _adapter;

    public DepartmentRepository(
        AppDbContext context,
        DepartmentEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// 部署情報を全件取得する。
    /// </summary>
    /// <returns>部署情報の一覧。</returns>
    public async Task<List<Department>> SelectAllAsync()
    {
        var entities = await _context.Departments
            .ToListAsync();

        return entities
            .Select(entity => _adapter.ToDomain(entity))
            .ToList();
    }

    /// <summary>
    /// 部署UUIDに一致する部署情報を取得する。
    /// </summary>
    /// <param name="departmentUuid">部署UUID。</param>
    /// <returns>部署情報。存在しない場合はnull。</returns>
    public async Task<Department?> SelectByUuidAsync(string departmentUuid)
    {
        var entity = await _context.Departments
            .FirstOrDefaultAsync(department =>
                department.DepartmentUuid == departmentUuid);

        if (entity is null)
        {
            return null;
        }

        return _adapter.ToDomain(entity);
    }
}