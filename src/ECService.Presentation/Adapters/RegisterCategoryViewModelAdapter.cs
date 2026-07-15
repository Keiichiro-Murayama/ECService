using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
namespace ECService.Presentation.Adapters;
/// <summary>
/// RegisterProductViewModelからドメインオブジェクト:Productへ変換するアダプタ
/// </summary>
public class RegisterCategoryViewModelAdapter : IRestorer<ProductCategory, RegisterCategoryRequest>
{
    /// <summary>
    /// RegisterProductViewModelからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">ユースケース:[新商品を登録する]を実現するViewModel</param>
    /// <returns></returns>
    public Task<ProductCategory> RestoreAsync(RegisterCategoryRequest target)
    {

        // 商品カテゴリを生成する
        var category = ProductCategory.Create(
                                        target.CategoryName!
                                    );
        // 商品カテゴリと商品在庫を設定する
        return Task.FromResult(category);
    }
}