using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 商品検索ユースケース
/// </summary>
public class SearchProductsUsecase : ISearchProductsUsecase
{
    /// <summary>
    /// 商品リポジトリ
    /// </summary>
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
    /// 商品検索を実行する
    /// </summary>
    /// <param name="categoryUuid">
    /// 商品カテゴリUUID。
    /// 指定されない場合は全商品を検索する。
    /// </param>
    /// <returns>商品一覧</returns>
    public async Task<List<Product>> ExecuteAsync(string? categoryUuid)
    {
        // categoryUuid が null・空文字・空白の場合は全商品を取得する
        if (string.IsNullOrWhiteSpace(categoryUuid))
        {
            return await _productRepository.SelectAllAsync();
        }

        // categoryUuid が指定されている場合はカテゴリで絞り込む
        return await _productRepository.SelectByCategoryAsync(categoryUuid);
    }
}