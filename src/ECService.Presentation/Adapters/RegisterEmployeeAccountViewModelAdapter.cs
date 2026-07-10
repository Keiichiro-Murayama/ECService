using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
namespace ECService.Presentation.Adapters;
/// <summary>
/// RegisterUserViewModelからドメインオブジェクト:Userへ変換するアダプタ
/// </summary> 
public class RegisterEmployeeAccountViewModelAdapter : IRestorer<EmployeeAcccount, RegisterEmployeeAccountViewModel>
{
    /// <summary>
    /// RegisterUserViewModelからドメインオブジェクト:Userを復元する
    /// </summary>
    /// <param name="target">ユースケース:[ユーザーを登録する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<EmployeeAcccount> RestoreAsync(RegisterEmployeeAccountViewModel target)
    {
        var employeeAccount = new EmployeeAcccount(target.Username, target.Email, target.Password);
        return Task.FromResult(user);
    }
}