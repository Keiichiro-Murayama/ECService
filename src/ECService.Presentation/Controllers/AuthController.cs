// using ECService.Application.Exceptions;
// using ECService.Application.Usecases.Interfaces;
// using ECService.Domain.Adapters;
// using ECService.Presentation.Adapters;
// using ECService.Presentation.ViewModels;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// namespace ECService.Presentation.Controllers;
// /// <summary>
// /// 認証(ログイン・ログアウト)に関する API を提供する
// /// </summary>
// [ApiController]
// [Route("library/api/auth")]
// [Tags("認証")]
// public class AuthController : ControllerBase
// {
//     /// <summary>認証トークンを格納する Cookie のキー名</summary>
//     private const string AuthCookieName = "access_token";

//     private readonly ILoginUsecase _loginUsecase;
//     private readonly LoginViewModelAdapter _loginRequestAdapter;

//     public AuthController(
//         ILoginUsecase loginUsecase,
//         LoginViewModelAdapter loginRequestAdapter)
//     {
//         _loginUsecase = loginUsecase;
//         _loginRequestAdapter = loginRequestAdapter;
//     }

//     /// <summary>
//     /// ログインする
//     /// POST /library/api/auth/login
//     /// </summary>
//     /// <param name="request"></param>
//     /// <returns></returns>
//     [HttpPost("login")]
//     public async Task<ActionResult> Login([FromBody] LoginRequest model)
//     {
//         // 入力検証
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         var input = _loginRequestAdapter.Restore(model);

//         try
//         {
//             var result = await _loginUsecase.ExecuteAsync(input);

//             // 簡易的なセッション固定攻撃対策:
//             // セッションミドルウェアを使っている場合は既存セッションをクリアし、
//             // セッションCookie を削除して新しいセッションID を発行させる。
//             try
//             {
//                 // Clear が安全に呼べるなら呼ぶ
//                 HttpContext.Session?.Clear();
//                 // 標準のセッションCookie 名(.AspNetCore.Session)を削除して新ID発行を促す
//                 Response.Cookies.Delete(".AspNetCore.Session");
//             }
//             catch
//             {
//                 // セッション未導入などの場合は何もしない(簡易版)
//             }

//             // 既存の認証Cookie を明示的に削除してから新しいトークンをセット
//             Response.Cookies.Delete(AuthCookieName);

//             var cookieOptions = new CookieOptions
//             {
//                 HttpOnly = true,
//                 Secure = Request.IsHttps, // 本番では true にする
//                 SameSite = SameSiteMode.Strict,
//                 Expires = DateTimeOffset.UtcNow.AddMinutes(60),
//             };

//             Response.Cookies.Append(AuthCookieName, result.AccessToken, cookieOptions);

//             return Ok(new LoginResponse
//             {
//                 AccountUuid = result.EmployeeAccount?.AccountUuid ?? string.Empty,
//                 AccountName = result.EmployeeAccount?.AccountName ?? string.Empty,
//                 Message = "ログインに成功しました。"
//             });
//         }
//         catch (AuthenticationException)
//         {
//             // 認証失敗は 401 に変換
//             return Unauthorized(new LoginResponse { Message = "ユーザー名またはパスワードが正しくありません。" });
//         }
//     }


//     /// <summary>
//     /// ログアウトする
//     /// POST /library/api/auth/logout
//     /// </summary>
//     /// <returns>成功メッセージ(200 OK)</returns>
//     [HttpPost("logout")]
//     [Authorize]   // ログアウトは認証が必要(ログイン中のユーザーが対象)
//     public ActionResult<LogoutResponse> Logout()
//     {
//         // 認証トークンの Cookie を削除する(以降のリクエストで送られなくなる)
//         Response.Cookies.Delete(AuthCookieName);

//         return Ok(new LogoutResponse { Message = "ログアウトしました。" });
//     }
// }