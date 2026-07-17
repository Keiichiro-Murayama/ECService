namespace ECService.Presentation.ViewModels;
/// <summary>
/// ログインのレスポンスを表す ViewModel(UC-02)
///
/// ログイン結果のメッセージを返す。認証トークンは HttpOnly Cookie で扱うため、本文には含めない。
/// </summary>
public class TokenResponse
{
    public string Token { get; set; } = string.Empty;

    public string AccountUuid { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}