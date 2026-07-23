using System.ComponentModel.DataAnnotations;

namespace ECService.Presentation.ViewModels;

/// <summary>
/// 商品修正APIのリクエスト。
/// </summary>
public class UpdateProductRequest
{
    /// <summary>
    /// 商品名。
    /// </summary>
    [Required(ErrorMessage = "商品名を入力してください")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "商品名は2～20文字以内で入力してください。")]
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// 価格。
    /// </summary>
    [Required(ErrorMessage = "価格を入力してください")]
    [Range(0, 1000000, ErrorMessage = "価格は100万円以下で入力してください")]
    public int? Price { get; set; }

    /// <summary>
    /// 在庫数。
    /// </summary>
    [Required(ErrorMessage = "在庫数を入力してください")]
    [Range(0, 1000, ErrorMessage = "在庫数は1000個以下で入力してください")]
    public int? Stock { get; set; }

    /// <summary>
    /// 商品カテゴリUUID。
    /// </summary>
    [Required(ErrorMessage = "カテゴリを選択してください")]
    public string CategoryUuid { get; set; } = string.Empty;

    /// <summary>
    /// 画像URL。
    /// </summary>
    [Required(ErrorMessage = "画像をアップロードしてください")]
    public string ImageUrl { get; set; } = string.Empty;
}