using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
namespace ECService.Presentation.Adapters;
/// <summary>
/// LoginDto と LoginRequest(ViewModel)を変換する Adapter
///
/// ・Restore : ViewModel → DTO(リクエストを受け取り、UseCase へ渡す DTO を組み立てる際に使用)
/// ・Convert : DTO → ViewModel。入力専用のため、現状サポートしない。
/// </summary>
public class LoginViewModelAdapter : IConverter<(string Token, EmployeeAccount EmployeeAccount), TokenResponse>
{


    public async Task<TokenResponse> ConvertAsync(
   (string Token, EmployeeAccount EmployeeAccount) domain)
    {
        return new TokenResponse
        {
            Token = domain.Token,
            AccountUuid = domain.EmployeeAccount.AccountUuid,
            AccountName = domain.EmployeeAccount.AccountName,
            Message = "ログインに成功しました。"
        };
    }
}