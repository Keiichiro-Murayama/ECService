using ECService.Domain.Models;

namespace ECService.Domain.Repositories;

/// <summary>
/// 商品カテゴリ情報を操作するRepositoryのインターフェース。
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// 商品カテゴリ情報を全件取得する。
    /// </summary>
    /// <returns>商品カテゴリ情報の一覧。</returns>
    Task<List<ProductCategory>> SelectAllAsync();

    /// <summary>
    /// 指定したカテゴリ名が既に存在するか確認する。
    /// </summary>
    /// <param name="name">カテゴリ名。</param>
    /// <returns>存在する場合true。</returns>
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// 商品カテゴリを登録する。
    /// </summary>
    /// <param name="productCategory">商品カテゴリ。</param>
    Task CreateAsync(ProductCategory productCategory);
}