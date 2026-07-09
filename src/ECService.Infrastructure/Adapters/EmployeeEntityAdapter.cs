using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 社員Entityからドメインモデルへ復元するAdapter。
/// </summary>
public class EmployeeEntityAdapter
{
    /// <summary>
    /// 社員Entityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="entity">社員Entity。</param>
    /// <returns>社員のドメインモデル。</returns>
    public Employee Restore(EmployeeEntity entity)
    {
        return new Employee(
            entity.EmployeeUuid,
            entity.Name,
            entity.Kana,
            entity.DepartmentId
        );
    }
}