using ECService.Domain.Models;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Adapters;

/// <summary>
/// 商品ドメインオブジェクトを商品検索用ViewModelへ変換するアダプタ
/// </summary>
public class SearchProductsViewModelAdapter
{
    /// <summary>
    /// 商品リストを商品検索結果ViewModelへ変換する
    /// </summary>
    /// <param name="products">商品ドメインオブジェクトのリスト</param>
    /// <returns>商品検索結果ViewModel</returns>
    public SearchProductsResponse Convert(List<Product> products)
    {
        return new SearchProductsResponse
        {
            Products = products
                .Select(product => new ProductsItem
                {
                    ProductUuid = product.ProductUuid,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl
                })
                .ToList()
        };
    }
}