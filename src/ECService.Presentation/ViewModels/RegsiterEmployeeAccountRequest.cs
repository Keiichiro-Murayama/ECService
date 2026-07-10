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
    [Display(Name = "カテゴリID")]
    public int? EmployeeId { get; set; } 

    /// <summary>
    /// メールアドレス（最大100文字）
    /// </summary>
    [Required(ErrorMessage = "アカウント名を入力してください")]
     [StringLength(20, MinimumLength = 5
        , ErrorMessage = "アカウント名は{2}文字以上、{1}文字以内で入力してください")]

    public string AccountName { get; init; } = string.Empty;

    /// <summary>
    /// 平文パスワード（8〜12文字）
    /// </summary>
    [Required(ErrorMessage = "パスワードを入力してください")]
    [StringLength(20, MinimumLength = 5
        , ErrorMessage = "パスワードは{2}文字以上、{1}文字以内で入力してください")]
    public string Password { get; init; } = string.Empty;

    public override string ToString()
    {
        return $"EmployeeId={EmployeeId}, AccountName={AccountName}, Password={Password}";
    }
}