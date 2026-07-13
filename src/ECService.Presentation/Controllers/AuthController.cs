using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace ECService.Presentations.Controllers;
/// <summary>
/// 認証(ログイン・ログアウト)に関する API を提供する
/// </summary>
[ApiController]
[Route("/api/admin/login")]
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
        var input = (model.Username, model.Password);
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
