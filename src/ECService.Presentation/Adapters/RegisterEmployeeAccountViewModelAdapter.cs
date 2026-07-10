using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
namespace ECService.Presentation.Adapters;
/// <summary>
/// RegisterUserViewModelからドメインオブジェクト:Userへ変換するアダプタ
/// </summary> 
public class RegisterEmployeeAccountViewModelAdapter : IRestorer<EmployeeAccount, RegisterEmployeeAccountRequest>
{
    /// <summary>
    /// RegisterUserViewModelからドメインオブジェクト:Userを復元する
    /// </summary>
    /// <param name="target">ユースケース:[ユーザーを登録する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<EmployeeAccount> RestoreAsync(RegisterEmployeeAccountRequest target)
    {
        var employeeAccount = new EmployeeAccount(target.Username, target.AccountName, target.Password);
        return Task.FromResult(employeeAccount);
    }
}