using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 部署情報を操作するRepositoryのインターフェース。
/// </summary>
public interface IDepartmentRepository
{
    /// <summary>
    /// 部署情報を全件取得する。
    /// </summary>
    /// <returns>部署情報の一覧。</returns>
    Task<List<Department>> SelectAllAsync();

    /// <summary>
    /// 部署UUIDに一致する部署情報を取得する。
    /// </summary>
    /// <param name="departmentUuid">部署UUID。</param>
    /// <returns>部署情報。存在しない場合はnull。</returns>
    Task<Department?> SelectByUuidAsync(string departmentUuid);
}