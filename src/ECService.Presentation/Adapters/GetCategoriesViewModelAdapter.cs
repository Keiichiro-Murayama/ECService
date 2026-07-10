using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 商品カテゴリドメインオブジェクトをViewModelへ変換するアダプタ
/// </summary>
public class GetCategoriesViewModelAdapter
{
    /// <summary>
    /// カテゴリリストをViewModelへ変換する
    /// </summary>
    /// <param name="categories">カテゴリドメインオブジェクトのリスト</param>
    /// <returns>ViewModel</returns>
    public GetCategoriesResponse Convert(List<CategoriesItem> categories)
    {
        return new GetCategoriesResponse
        {
            Categories = categories
                .Select(categories => new CategoriesItem
                {
                    CategoryUuid = categories.CategoryUuid,
                    CategoryId = categories.CategoryId,
                    Name = categories.Name
                })
                .ToList()
        };
    }
}