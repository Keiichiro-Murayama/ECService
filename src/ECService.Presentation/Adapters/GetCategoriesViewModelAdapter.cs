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
    public async Task<GetCategoriesResponse> Convert(List<ProductCategory> categories)
    {
        return new GetCategoriesResponse
        {
            Categories = categories
                .Select(categories => new CategoriesItem
                {
                    CategoryUuid = categories.CategoryUuid,
                    Name = categories.Name
                })
                .ToList()
        };
    }
}