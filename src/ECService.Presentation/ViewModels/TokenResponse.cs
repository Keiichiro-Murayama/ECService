namespace ECService.Presentation.ViewModels;

/// <summary>
/// ログイン結果を返すViewModel
/// </summary>
public class TokenResponse
{
    public string Token { get; set; } = string.Empty;

    public string AccountUuid { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    //石原:追加
    public string EmployeeName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}