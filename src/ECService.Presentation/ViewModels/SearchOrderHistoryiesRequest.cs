using System.ComponentModel.DataAnnotations;

namespace ECService.Presentation.ViewModels;

//石原:追加
/// <summary>
/// 購入履歴検索APIの検索条件を表すViewModel
/// </summary>
public class SearchOrderHistoriesRequest
{
    /// <summary>
    /// 購入日
    /// 未指定の場合は購入日による絞り込みを行わない
    /// </summary>
    public DateOnly? PurchaseDate { get; set; }

    /// <summary>
    /// 顧客アカウント名
    /// 未指定の場合は顧客アカウント名による絞り込みを行わない
    /// </summary>
    [StringLength(
        30,
        ErrorMessage = "顧客アカウント名は30文字以内で入力してください。")]
    public string? CustomerAccountName { get; set; }
}