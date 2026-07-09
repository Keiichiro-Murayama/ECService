using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 商品カテゴリのドメインモデルとEntityを変換するAdapter。
/// </summary>
public class ProductCategoryEntityAdapter :
    IConverter<ProductCategory, ProductCategoryEntity>,
    IRestorer<ProductCategory, ProductCategoryEntity>
{
    /// <summary>
    /// 商品カテゴリのドメインモデルをEntityへ変換する。
    /// </summary>
    /// <param name="domain">商品カテゴリのドメインモデル。</param>
    /// <returns>商品カテゴリEntity。</returns>
    public Task<ProductCategoryEntity> ConvertAsync(ProductCategory domain)
    {
        var entity = new ProductCategoryEntity
        {
            CategoryUuid = Guid.Parse(domain.CategoryUuid),
            Name = domain.Name
        };

        return Task.FromResult(entity);
    }

    /// <summary>
    /// 商品カテゴリEntityからドメインモデルへ復元する。
    /// </summary>
    /// <param name="target">商品カテゴリEntity。</param>
    /// <returns>商品カテゴリのドメインモデル。</returns>
    public Task<ProductCategory> RestoreAsync(ProductCategoryEntity target)
    {
        var domain = new ProductCategory(
            target.CategoryUuid.ToString(),
            target.Name
        );

        return Task.FromResult(domain);
    }
}