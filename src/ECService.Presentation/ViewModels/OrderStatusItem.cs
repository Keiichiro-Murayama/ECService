namespace ECService.Presentation.ViewModels;

/// <summary>
/// 注文ステータスの選択肢1件分を表すViewModel
/// </summary>
public class OrderStatusItem
{
    /// <summary>
    /// 注文ステータスID
    /// </summary>
    public int OrderStatusId { get; set; }

    /// <summary>
    /// 注文ステータス名
    /// </summary>
    public string OrderStatusName { get; set; } = string.Empty;
}