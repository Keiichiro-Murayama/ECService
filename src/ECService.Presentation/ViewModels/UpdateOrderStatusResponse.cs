namespace ECService.Presentation.ViewModels;

/// <summary>
/// 注文ステータス更新結果を表すViewModel。
/// </summary>
public class UpdateOrderStatusResponse
{
    /// <summary>
    /// 注文UUID。
    /// </summary>
    public string OrderUuid { get; set; } = string.Empty;

    /// <summary>
    /// 更新後の注文ステータスID。
    /// </summary>
    public int OrderStatusId { get; set; }

    /// <summary>
    /// 更新後の注文ステータス名。
    /// </summary>
    public string OrderStatusName { get; set; } = string.Empty;

    /// <summary>
    /// 更新日時。
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
