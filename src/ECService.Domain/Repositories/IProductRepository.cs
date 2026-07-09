using ECService.Domain.Models;
namespace ECService.Domains.Repositories;

/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイス
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="Product">永続化する商品</param>
    /// <returns>なし</returns>
    Task CreateAsync(Product Product);

    /// <summary>
    /// 商品を更新する
    /// </summary>
    /// <param name="Product">更新対象の商品</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    Task<bool> UpdateAsync(Product Product);

    /// <summary>
    /// 指定された商品Idの商品と在庫、商品カテゴリを返す
    /// </summary>
    /// <param name="productUuid">商品Uuid</param>
    /// <returns>Product または null</returns>
    Task<Product?> SelectByUuidAsync(Guid productUuid);

    /// <summary>
    /// 商品を削除する
    /// </summary>
    /// <param name="productUuid">削除対象の商品Id(UUID)</param>
    /// <returns>true:削除成功 false:削除失敗</returns>
    Task<bool> DeleteAsync(Guid productUuid);

    /// <summary>
    /// 指定された商品名の存在有無を返す
    /// </summary>
    /// <param name="name">商品名</param>
    /// <returns>true:存在する false:存在しない</returns> 
    Task<bool> ExistsByNameAsync(string name);

    /// <summary>
    /// すべての商品を取得する
    /// </summary>

    Task<List<Product>>SelectAllAsync();

    /// <summary>
    /// カテゴリで商品一覧を取得する
    /// </summary>
    /// <param name="CategoryUuid">カテゴリーId</param>
    Task<List<Product>>SelectByCategoryAsync(Guid CategoryUuid);

}