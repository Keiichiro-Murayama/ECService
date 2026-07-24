namespace ECService.Presentation.ViewModels;

/// <summary>
/// 購入履歴検索結果の1件分を表すViewModel
/// </summary>
public class OrderHistoriesItem
{
    /// <summary>
    /// 注文UUID
    /// </summary>
    public string OrderUuid { get; set; } = string.Empty;

    /// <summary>
    /// 購入日時
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// 顧客アカウント名
    /// </summary>
    public string CustomerAccountName { get; set; } = string.Empty;

    /// <summary>
    /// 注文内容
    /// </summary>
    public string OrderContent { get; set; } = string.Empty;

    /// <summary>
    /// 注文ステータス
    /// </summary>
    public string OrderStatus { get; set; } = string.Empty;
}