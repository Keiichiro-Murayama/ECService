// using ECService.Application.Usecases.Interfaces;
// using ECService.Domain.Adapters;
// using ECService.Domain.Models;
// using ECService.Presentation.ViewModels;
// namespace ECService.Presentation.Adapters;
// /// <summary>
// /// LoginDto と LoginRequest(ViewModel)を変換する Adapter
// ///
// /// ・Restore : ViewModel → DTO(リクエストを受け取り、UseCase へ渡す DTO を組み立てる際に使用)
// /// ・Convert : DTO → ViewModel。入力専用のため、現状サポートしない。
// /// </summary>
// public class LoginViewModelAdapter : IAdapter<EmployeeAccount, LoginRequest>
// {
//     /// <summary>
//     /// DTO を ViewModel に変換する(DTO → ViewModel)。入力専用のため未サポート。
//     /// </summary>
//     /// <exception cref="NotSupportedException">本変換はサポートしていない</exception>
//     public LoginRequest Convert(EmployeeAccount employeeAccount)
//     {
//         throw new NotSupportedException(
//             "LoginDto から LoginRequest への変換はサポートしていません。");
//     }

//     /// <summary>
//     /// ViewModel を DTO に変換する(ViewModel → DTO)
//     /// </summary>
//     /// <param name="source">プレゼンテーション層の LoginRequest</param>
//     /// <returns>アプリケーション層の LoginDto</returns>
//     public LoginDto Restore(LoginRequest source)
//     {
//         return new LoginDto
//         {
//             Username = source.Username,
//             Password = source.Password,
//         };
//     }
// }