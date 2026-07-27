using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace ECService.Presentation.ViewModels;

/// <summary>
/// 商品登録ユースケース用ViewModelクラス
/// </summary>
public class RegisterProductRequest
{
    /// <summary>
    /// 商品名
    /// </summary>
    [Required(
        ErrorMessage =
            "商品名を入力してください")]
    [StringLength(
        20,
        MinimumLength = 2,
        ErrorMessage =
            "商品名は2～20文字以内で入力してください。")]
    [Display(Name = "商品名")]
    public string ProductName { get; set; }
        = string.Empty;

    /// <summary>
    /// 単価
    /// </summary>
    [Required(
        ErrorMessage =
            "価格を入力してください")]
    [Range(
        0,
        1_000_000,
        ErrorMessage =
            "価格は100万円以下で入力してください")]
    [Display(Name = "単価")]
    public int? Price { get; set; }

    /// <summary>
    /// 在庫数
    /// </summary>
    [Required(
        ErrorMessage =
            "在庫数を入力してください")]
    [Range(
        0,
        1_000,
        ErrorMessage =
            "在庫数は1000個以下で入力してください")]
    [Display(Name = "在庫数")]
    public int? Stock { get; set; }

    /// <summary>
    /// 商品カテゴリUUID
    /// </summary>
    [Required(
        ErrorMessage =
            "カテゴリを選択してください")]
    [Display(Name = "カテゴリUUID")]
    public string CategoryUuid { get; set; }
        = string.Empty;

    /// <summary>
    /// 登録する商品画像
    /// </summary>
    //石原:変更 商品画像をmultipart/form-dataで受け取る
    [Required(
        ErrorMessage =
            "画像をアップロードしてください")]
    [Display(Name = "商品画像")]
    public IFormFile? Image { get; set; }
}