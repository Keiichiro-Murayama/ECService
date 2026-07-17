using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domain;
using ECService.Domain.Models;
using Microsoft.Extensions.Validation;
namespace ECService.Presentation.ViewModels;

public class GetCategoriesResponse
{
    /// <summary>
    /// カテゴリリスト
    /// </summary>
    public List<CategoriesItem>? Categories { get; set; }
}