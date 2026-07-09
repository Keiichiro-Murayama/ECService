using ECService.Domain.Models;
namespace ECService.Application.Usecases.Interfaces;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するインターフェイス
/// </summary>
public interface IRegisterEmployeeAccountUsecase
{
    /// <summary>
    /// ユーザーを登録する
    /// </summary>
    /// <param name="employeeAcount">登録対象ユーザー</param>
    /// <returns></returns>
    Task ExecuteAsync(EmployeeAccount employeeAccount);
}