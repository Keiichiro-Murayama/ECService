using System.ComponentModel.DataAnnotations;
namespace ECService.Presentation.ViewModels;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するViewModel
/// </summary>
public class RegisterEmployeeAccountRequest
{
    /// <summary>
    /// 選択された社員Id
    /// </summary>
    [Required(ErrorMessage = "社員名を選択してください")]
    [Display(Name = "社員ID")]
    public string EmployeeUuid { get; set; }

    /// <summary>
    /// アカウント名（最大20文字）
    /// </summary>
    [Required(ErrorMessage = "アカウント名を入力してください")]
    [StringLength(20, MinimumLength = 5
        , ErrorMessage = "アカウント名は{2}文字以上、{1}文字以内で入力してください")]
    [RegularExpression(
        @"^[a-zA-Z0-9]+$",
        ErrorMessage = "アカウント名は半角英数字で入力してください")]
    public string AccountName { get; init; } = string.Empty;

    /// <summary>
    /// 平文パスワード（5〜20文字）
    /// </summary>
    [Required(ErrorMessage = "パスワードを入力してください")]
    [StringLength(20, MinimumLength = 5
        , ErrorMessage = "パスワードは{2}文字以上、{1}文字以内で入力してください")]
    [RegularExpression(
        @"^[a-zA-Z0-9]+$",
        ErrorMessage = "パスワードは半角英数字で入力してください")]
    public string Password { get; init; } = string.Empty;


}