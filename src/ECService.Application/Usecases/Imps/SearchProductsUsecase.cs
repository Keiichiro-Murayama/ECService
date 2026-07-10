using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 商品検索ユースケースの実装クラス
/// </summary>
public class SearchProductsUsecase : ISearchProductsUsecase
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="productRepository">商品リポジトリ</param>
    public SearchProductsUsecase(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    /// <summary>
    /// 商品を検索する
    /// </summary>
    /// <param name="categoryUuid">検索対象の商品カテゴリUUID</param>
    /// <returns>検索結果の商品一覧</returns>
    public async Task<List<Product>> ExecuteAsync(string? categoryUuid)
    {
        // カテゴリUUIDが未指定の場合は全商品を取得する
        if (string.IsNullOrWhiteSpace(categoryUuid))
        {
            return await _productRepository.SelectAllAsync();
        }

        // カテゴリUUIDが指定されている場合はカテゴリ別に商品を取得する
        return await _productRepository.SelectByCategoryAsync(categoryUuid);
    }
}