namespace ECService.Presentation.ViewModels;

/// <summary>
/// 購入履歴検索結果を表すViewModel
/// </summary>
public class SearchOrderHistoriesResponse
{
    /// <summary>
    /// 購入履歴一覧
    /// </summary>
    public List<OrderHistoriesItem> OrderHistories { get; set; } = new();
}