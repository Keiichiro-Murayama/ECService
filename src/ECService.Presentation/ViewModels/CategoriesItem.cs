using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domain;
using ECService.Domain.Models;
using Microsoft.Extensions.Validation;
namespace ECService.Presentation.ViewModels;
/// <summary>
/// 商品登録ユースケース用ViewModelクラス
/// </summary>
public class CategoriesItem
{
    public string? CategoryUuid;
    public string? CategoryId;
    public string? Name;
}