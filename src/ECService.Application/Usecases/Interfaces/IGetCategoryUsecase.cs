using ECService.Domain.Models;
namespace ECService.Application.Usecases.Interfaces;
/// <summary>
/// 商品登録サービスインターフェイス
/// </summary>
public interface IGetCategoryUsecase
{
    /// <summary>
    /// カテゴリ情報を取得する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    Task<List<ProductCategory>> ExecuteAsync();
}