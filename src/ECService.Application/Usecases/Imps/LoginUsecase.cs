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
        // ユーザー名でユーザーを検索する
        var employeeAccount = await _employeeAccountRepository.SelectByAccountNameAsync(input.Username);

        // ユーザーが存在しない場合も、パスワードが一致しない場合も、
        // 区別せずに同一の認証失敗として扱う(列挙攻撃を防ぐ:UC-02 BR-04)
        if (employeeAccount is null)
        {
            throw new AuthenticationException(
                "AuthenticationFailed", "ユーザー名またはパスワードが正しくありません。");
        }

        // LoginInteractor の Verify 呼び出しを修正
        var isValid = _passwordService.Verify(employeeAccount.PasswordHash, input.Password);
        if (!isValid)
        {
            throw new AuthenticationException(
                "AuthenticationFailed", "ユーザー名またはパスワードが正しくありません。");
        }

        // 認証成功 → JWT を発行する
        var accessToken = _jwtTokenProvider.IssueAccessToken(employeeAccount);

        return (accessToken, employeeAccount);
    }
}