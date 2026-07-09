using ECService.Domain.Models;
namespace ECService.Applications.Usecases.Interfaces;
/// <summary>
/// 商品登録サービスインターフェイス
/// </summary>
public interface IRegisterProductUsecase
{
    /// <summary>
    /// 商品を新規登録する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    Task ExecuteAsync(Product product);
}