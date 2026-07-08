using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 社員情報を操作するRepositoryのインターフェース。
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// 社員情報を全件取得する。
    /// </summary>
    /// <returns>社員情報の一覧。</returns>
    Task<List<Employee>> SelectAllAsync();

    /// <summary>
    /// 社員UUIDに一致する社員情報を取得する。
    /// </summary>
    /// <param name="employeeUuid">社員UUID。</param>
    /// <returns>社員情報。存在しない場合はnull。</returns>
    Task<Employee?> SelectByUuidAsync(string employeeUuid);
}