using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domains;
using ECService.Domain.Models;
using Microsoft.Extensions.Validation;
namespace ECService.Presentations.ViewModels;

public class GetCategoryRequest
{
    /// <summary>
    /// カテゴリリスト
    /// </summary>
    public List<CategoriesItem>? Categories { get; set; }
}