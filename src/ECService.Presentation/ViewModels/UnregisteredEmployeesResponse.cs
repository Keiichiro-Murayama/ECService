using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECService.Domain;
using ECService.Domain.Models;
using Microsoft.Extensions.Validation;
namespace ECService.Presentation.ViewModels;

public class UnregisteredEmployeesResponse
{
    /// <summary>
    /// カテゴリリスト
    /// </summary>
    public List<UnregisteredEmployeesItem>? Employees { get; set; }
}