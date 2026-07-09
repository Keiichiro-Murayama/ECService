using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 担当者アカウントのドメインモデルとEntityを変換するAdapter。
/// </summary>
public class EmployeeAccountEntityAdapter
{
    /// <summary>
    /// 担当者アカウントのドメインモデルをEntityへ変換する。
    /// </summary>
    /// <param name="account">担当者アカウントのドメインモデル。</param>
    /// <returns>担当者アカウントEntity。</returns>
    public EmployeeAccountEntity Convert(EmployeeAccount account)
    {
        return new EmployeeAccountEntity
        {
            AccountUuid = account.AccountUuid,
            Name = account.Name,
            Password = account.Password,
            EmployeeId = account.EmployeeId
        };
    }

    /// <summary>
    /// 担当者アカウントEntityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="entity">担当者アカウントEntity。</param>
    /// <returns>担当者アカウントのドメインモデル。</returns>
    public EmployeeAccount Restore(EmployeeAccountEntity entity)
    {
        return new EmployeeAccount(
            entity.AccountUuid,
            entity.Name,
            entity.Password,
            entity.EmployeeId
        );
    }
}