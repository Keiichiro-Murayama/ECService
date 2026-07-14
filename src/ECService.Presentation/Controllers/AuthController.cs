using ECService.Application.Usecases.Interfaces;
using ECService.Application.Exceptions;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ECService.Presentations.Controllers;
/// <summary>
/// 認証(ログイン・ログアウト)に関する API を提供する
/// </summary>
[ApiController]
[Route("/api/admin")]
[Tags("認証")]
public class AuthController : ControllerBase
{
    /// <summary>認証トークンを格納する Cookie のキー名</summary>
    private const string AuthCookieName = "access_token";

    private readonly ILoginUsecase _loginUsecase;

    public AuthController(
        ILoginUsecase loginUsecase)
    {
        _loginUsecase = loginUsecase;}

    /// <summary>
    /// ログインする
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest model)
    {
        try
        {
            var input = (model.Username, model.Password);
            // ★このユースケース実行で例外が発生する可能性があるため、tryの中で実行します
            var result = await _loginUsecase.ExecuteAsync(input);

            // 発行された JWT を HttpOnly Cookie にセットする
            Response.Cookies.Append(AuthCookieName, result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,                  // ★開発はHTTPのため false
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(60),
            });

            return Ok(new TokenResponse { Message = "ログインに成功しました。" });
        }
        catch (AuthenticationException ex)
        {
            // 1. アカウントロック状態の場合 (HTTP 423 Locked)
            if (ex.ErrorCode == "AccountLocked")
            {
                return StatusCode(StatusCodes.Status423Locked, new
                {
                    error = ex.ErrorCode,
                    message = ex.Message,
                    remainingMinutes = ex.RemainingMinutes // 画面表示用に残り時間を返す
                });
            }

            // 2. 通常の認証失敗（パスワード間違いなど）の場合 (HTTP 401 Unauthorized)
            return Unauthorized(new
            {
                error = ex.ErrorCode,
                message = ex.Message
            });
        }
    }


    /// <summary>
    /// ログアウトする
    /// 
    /// </summary>
    /// <returns>成功メッセージ(200 OK)</returns>
[HttpPost("logout")]
[Authorize]
public IActionResult Logout()
{
    Response.Cookies.Delete("access_token");

    return Ok(new
    {
        Message = "ログアウトしました。"
    });
}

    [Authorize]
[HttpGet("test")]
public IActionResult Test()
{
    return Ok("ログインしています");
}
}
