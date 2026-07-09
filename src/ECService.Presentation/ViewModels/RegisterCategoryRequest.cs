using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domains;
using ECService.Domain.Models;
using Microsoft.Extensions.Validation;
namespace ECService.Presentations.ViewModels;
/// <summary>
/// 商品登録ユースケース用ViewModelクラス
/// </summary>
public class RegisterCategoryRequest
{
    /// <summary>
    /// 商品名
    /// </summary>
    [Required(ErrorMessage = "カテゴリ名を入力してください")]
    [StringLength(30,ErrorMessage = "カテゴリ名は30文字以内で入力してください。")]
    [Display(Name = "カテゴリ名")]
    public string? categoryName { get; set; }
}