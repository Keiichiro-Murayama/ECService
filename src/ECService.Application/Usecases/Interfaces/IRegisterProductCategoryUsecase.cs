using ECService.Domain.Models;
namespace ECService.Applications.Usecases.Interfaces;
/// <summary>
/// 商品カテゴリ登録サービスインターフェイス
/// </summary>
public interface IRegisterProductCategoryUsecase
{
    /// <summary>
    /// 商品カテゴリを新規登録する
    /// </summary>
    /// <param name="productCategory">永続化する商品カテゴリ</param>
    Task ExecuteAsync(ProductCategory productCategory);
}