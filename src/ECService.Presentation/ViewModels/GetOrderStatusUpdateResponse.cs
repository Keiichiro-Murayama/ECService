namespace ECService.Presentation.ViewModels;

/// <summary>
/// 注文ステータス更新画面の表示情報を表すViewModel
/// </summary>
public class GetOrderStatusUpdateResponse
{
    /// <summary>
    /// 注文UUID
    /// </summary>
    public string OrderUuid { get; set; } = string.Empty;

    /// <summary>
    /// 購入日時
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 顧客アカウント名
    /// </summary>
    public string CustomerAccountName { get; set; } = string.Empty;

    /// <summary>
    /// 注文内容
    /// </summary>
    public string OrderContent { get; set; } = string.Empty;

    /// <summary>
    /// 現在の注文ステータスID
    /// </summary>
    public int CurrentOrderStatusId { get; set; }

    /// <summary>
    /// 現在の注文ステータス名
    /// </summary>
    public string CurrentOrderStatusName { get; set; } = string.Empty;

    /// <summary>
    /// 選択可能な注文ステータス一覧
    /// </summary>
    public List<OrderStatusItem> OrderStatuses { get; set; } = new();
}