using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 担当者アカウントのドメインモデルとEntityを変換するAdapter。
/// </summary>
public class EmployeeAccountEntityAdapter :
    IConverter<EmployeeAccount, EmployeeAccountEntity>,
    IRestorer<EmployeeAccount, EmployeeAccountEntity>
{
    /// <summary>
    /// 担当者アカウントのドメインモデルをEntityへ変換する。
    /// </summary>
    public Task<EmployeeAccountEntity> ConvertAsync(EmployeeAccount domain)
    {
        var entity = new EmployeeAccountEntity
        {
            AccountUuid = Guid.Parse(domain.AccountUuid),
            Name = domain.AccountName,
            Password = domain.PasswordHash
        };

        return Task.FromResult(entity);
    }

    /// <summary>
    /// 担当者アカウントEntityからドメインモデルへ復元する。
    /// </summary>
    public Task<EmployeeAccount> RestoreAsync(EmployeeAccountEntity target)
    {
        var department = new Department(
            target.Employee.Department.DepartmentUuid.ToString(),
            target.Employee.Department.Name
        );

        var employee = new Employee(
            target.Employee.EmployeeUuid.ToString(),
            target.Employee.Name,
            target.Employee.Kana,
            department
        );

        var account = new EmployeeAccount(
            target.AccountUuid.ToString(),
            target.Name,
            target.Password,
            target.Password,
            employee
        );

        return Task.FromResult(account);
    }
}