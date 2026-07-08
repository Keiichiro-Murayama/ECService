using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 担当者アカウント情報を操作するRepositoryのインターフェース。
/// </summary>
public interface IEmployeeAccountRepository
{
    /// <summary>
    /// アカウント名に一致する担当者アカウントを取得する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>担当者アカウント。存在しない場合はnull。</returns>
    Task<EmployeeAccount?> SelectByAccountNameAsync(string accountName);

    /// <summary>
    /// 指定したアカウント名が既に存在するか確認する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>存在する場合true。</returns>
    Task<bool> ExistsByAccountNameAsync(string accountName);

    /// <summary>
    /// 担当者アカウントを登録する。
    /// </summary>
    /// <param name="employeeAccount">担当者アカウント。</param>
    Task CreateAsync(EmployeeAccount employeeAccount);
}