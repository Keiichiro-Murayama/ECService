namespace ECService.Presentation.ViewModels;

/// <summary>
/// 商品検索結果を表すViewModel
/// </summary>
public class SearchProductsResponse
{
    /// <summary>
    /// 商品一覧
    /// </summary>
    public List<ProductsItem> Products { get; set; } = new();
}