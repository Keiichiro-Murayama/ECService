namespace ECService.Presentation.ViewModels;

/// <summary>
/// 商品検索結果の商品1件分を表すViewModel
/// </summary>
public class ProductsItem
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; set; } = string.Empty;

    /// <summary>
    /// 商品名
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 単価
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// 商品画像URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;
}