using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 商品カテゴリドメインオブジェクトをViewModelへ変換するアダプタ
/// </summary>
public class UnregisteredEmployeesViewModelAdapter
{
    /// <summary>
    /// カテゴリリストをViewModelへ変換する
    /// </summary>
    /// <param name="unregistered">カテゴリドメインオブジェクトのリスト</param>
    /// <returns>ViewModel</returns>
    public async Task<UnregisteredEmployeesResponse> Convert(List<Employee> unregistered)
    {
        return new UnregisteredEmployeesResponse
        {
            Employees = unregistered
                .Select(unregistered => new UnregisteredEmployeesItem
                {
                    EmployeeUuid = unregistered.EmployeeUuid,
                    AccountName = unregistered.Name,
                })
                .ToList()
        };
    }
}