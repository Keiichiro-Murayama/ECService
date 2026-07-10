using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 商品削除ユースケース
/// </summary>
public class DeleteProductUsecase : IDeleteProductUsecase
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">商品リポジトリ</param>
    public DeleteProductUsecase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 指定された商品を削除する
    /// </summary>
    /// <param name="productUuid">削除対象の商品UUID</param>
    public async Task ExecuteAsync(string productUuid)
    {
        var result = await _productRepository.DeleteAsync(productUuid);

        if (!result)
        {
            throw new InvalidOperationException(
                "削除対象の商品が見つかりませんでした。");
        }
    }
}