using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domains;
using ECService.Domain.Models;
namespace ECService.Presentations.ViewModels;
/// <summary>
/// 商品登録ユースケース用ViewModelクラス
/// </summary>
public class RegisterProductViewModel
{
    /// <summary>
    /// 商品名
    /// </summary>
    [Required(ErrorMessage = "商品名は必須です。")]
    [StringLength(30, ErrorMessage = "商品名は30文字以内で入力してください。")]
    [Display(Name = "商品名")]
    public string? productName { get; set; }

    /// <summary>
    /// 単価
    /// </summary>
    [Required(ErrorMessage = "単価は必須です。")]
    [Range(0, int.MaxValue, ErrorMessage = "単価は0以上で入力してください。")]
    [Display(Name = "単価")]
    public int? price { get; set; }

    /// <summary>
    /// 在庫数
    /// </summary>
    [Required(ErrorMessage = "在庫数は必須です。")]
    [Range(0, int.MaxValue, ErrorMessage = "在庫数は0以上で入力してください。")]
    [Display(Name = "在庫数")]
    public int? Stock { get; set; }

    /// <summary>
    /// 選択された商品カテゴリId
    /// </summary>
    [Required(ErrorMessage = "商品カテゴリを選択してください。")]
    [Display(Name = "商品カテゴリ")]
    public int? CategoryId { get; set; } 

}