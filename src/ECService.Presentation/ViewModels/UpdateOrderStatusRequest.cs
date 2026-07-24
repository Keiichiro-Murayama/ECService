using System.ComponentModel.DataAnnotations;

namespace ECService.Presentation.ViewModels;

/// <summary>
/// 注文ステータス更新APIのリクエスト。
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>
    /// 変更後の注文ステータスID。
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage = "注文ステータスIDは1以上で指定してください。")]
    public int OrderStatusId { get; set; }
}