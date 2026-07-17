using ECService.Application.Authentications;
using ECService.Domain.Models;
using ECService.Application.Exceptions;
using ECService.Domain.Repositories;
using ECService.Application.Usecases.Interfaces;
namespace ECService.Application.Usecases.Imps;

/// <summary>
/// ILoginUseCase の実装(Interactor)
///
/// ユーザー名でユーザーを検索し、パスワードを照合する。
/// 認証に成功すれば JWT を発行して返す。
/// 認証に失敗する場合(ユーザーが存在しない、またはパスワードが一致しない)は、
/// それらを区別せず、同一の認証失敗例外をスローする(列挙攻撃を防ぐため:UC-02 BR-04)。
/// 読み取りのみのため、トランザクションは用いない。
/// </summary>
public class LoginUsecase : ILoginUsecase
{
    private readonly IEmployeeAccountRepository _employeeAccountRepository;
    private readonly IPasswordService _passwordService;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LoginUsecase(
        IEmployeeAccountRepository employeeAccountRepository,
        IPasswordService passwordService,
        IJwtTokenProvider jwtTokenProvider)
    {
        _employeeAccountRepository = employeeAccountRepository;
        _passwordService = passwordService;
        _jwtTokenProvider = jwtTokenProvider;
    }

    /// <summary>
    /// ユーザー名とパスワードで認証し、成功すれば JWT を発行する
    /// </summary>
    /// <param name="input">ログインの入力情報</param>
    /// <returns>発行された JWT を含むログイン結果</returns>
    /// <exception cref="AuthenticationFailedException">認証に失敗した場合</exception>
    public async Task<(string AccessToken, EmployeeAccount EmployeeAccount)> ExecuteAsync((string Username, string Password) input)
{
    // 1. ユーザー名でユーザーを検索する
    var employeeAccount = await _employeeAccountRepository.SelectByAccountNameAsync(input.Username);

    if (employeeAccount is null)
    {
        throw new AuthenticationException(
            "AuthenticationFailed", "ユーザー名またはパスワードが正しくありません。");
    }

    // 現在アカウントがロック中かどうかを判定
    var now = DateTime.UtcNow;
//     Console.WriteLine($"[DEBUG] DBのLockoutEnd: {employeeAccount.LockoutEnd}, 現在時刻(Utc): {now}");
//     if (employeeAccount.LockoutEnd.HasValue)
// {
//     // ★検証用ログ：どっちが大きいか出す
//     Console.WriteLine($"[DEBUG] ロック終了時刻 > 現在時刻 か？ : {employeeAccount.LockoutEnd.Value > now}");
// }
    if (employeeAccount.LockoutEnd.HasValue && employeeAccount.LockoutEnd.Value > now)
    {
        var remainingMinutes = Math.Ceiling((employeeAccount.LockoutEnd.Value - now).TotalMinutes);
        throw new AuthenticationException(
                "AccountLocked", 
                $"アカウントはロックされています。残り約 {remainingMinutes} 分後に再度お試しください。", 
                remainingMinutes
            );
    }

    // 3. パスワードの検証
    var isValid = _passwordService.Verify(employeeAccount.PasswordHash, input.Password);
    
    if (!isValid)
    {
        // 失敗回数のカウントアップやロック処理
        await _employeeAccountRepository.RecordLoginFailureAsync(input.Username);

        throw new AuthenticationException(
            "AuthenticationFailed", "ユーザー名またはパスワードが正しくありません。");
    }

    // 認証成功時は、次回のためにリポジトリ経由で失敗カウントをリセットする
    await _employeeAccountRepository.ResetLoginFailureAsync(input.Username);

    // 5. 認証成功 → JWT を発行する
    var accessToken = _jwtTokenProvider.IssueAccessToken(employeeAccount);

    return (accessToken, employeeAccount);
}

}