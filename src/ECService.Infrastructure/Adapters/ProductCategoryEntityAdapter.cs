using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 商品カテゴリのドメインモデルとEntityを変換するAdapter。
/// </summary>
public class ProductCategoryEntityAdapter
{
    /// <summary>
    /// 商品カテゴリのドメインモデルをEntityへ変換する。
    /// </summary>
    /// <param name="category">商品カテゴリのドメインモデル。</param>
    /// <returns>商品カテゴリEntity。</returns>
    public ProductCategoryEntity Convert(ProductCategory category)
    {
        return new ProductCategoryEntity
        {
            CategoryUuid = Guid.Parse(category.CategoryUuid),
            Name = category.Name
        };
    }

    /// <summary>
    /// 商品カテゴリEntityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="entity">商品カテゴリEntity。</param>
    /// <returns>商品カテゴリのドメインモデル。</returns>
    public ProductCategory Restore(ProductCategoryEntity entity)
    {
        return ProductCategory.Restore(
            entity.CategoryUuid.ToString(),
            entity.Name
        );
    }
}