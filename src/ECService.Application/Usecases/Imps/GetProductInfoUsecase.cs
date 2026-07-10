using ECService.Applications.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Applications.Usecases.Imps;

/// <summary>
/// 商品詳細取得ユースケース。
/// </summary>
public class GetProductInfoUsecase : IGetProductInfoUsecase
{
    private readonly IProductRepository _productRepository;

    public GetProductInfoUsecase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 商品UUIDに一致する商品情報を取得する。
    /// </summary>
    /// <param name="productUuid">商品UUID。</param>
    /// <returns>商品情報。存在しない場合はnull。</returns>
    public async Task<Product?> ExecuteAsync(string productUuid)
    {
        return await _productRepository.SelectByUuidAsync(productUuid);
    }
}