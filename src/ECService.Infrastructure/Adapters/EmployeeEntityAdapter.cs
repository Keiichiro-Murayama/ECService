using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 社員Entityからドメインモデルへ復元するAdapter。
/// </summary>
public class EmployeeEntityAdapter :
    IRestorer<Employee, EmployeeEntity>
{
    /// <summary>
    /// 社員Entityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="target">社員Entity。</param>
    /// <returns>社員のドメインモデル。</returns>
    public Task<Employee> RestoreAsync(EmployeeEntity target)
    {
        var department = new Department(
            target.Department.DepartmentUuid.ToString(),
            target.Department.Name
        );

        var employee = new Employee(
            target.EmployeeUuid.ToString(),
            target.Name,
            target.Kana,
            department
        );

        return Task.FromResult(employee);
    }
}