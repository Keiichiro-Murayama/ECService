using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 商品カテゴリ情報を操作するRepository。
/// </summary>
public class ProductCategoryRepository : IProductCategoryRepository
{
    private readonly AppDbContext _context;
    private readonly ProductCategoryEntityAdapter _adapter;

    public ProductCategoryRepository(
        AppDbContext context,
        ProductCategoryEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// 商品カテゴリ情報を全件取得する。
    /// </summary>
    /// <returns>商品カテゴリ情報の一覧。</returns>
    public async Task<List<ProductCategory>> SelectAllAsync()
    {
        var entities = await _context.ProductCategories
            .ToListAsync();

        var productCategories = new List<ProductCategory>();

        foreach (var entity in entities)
        {
            var productCategory = await _adapter.RestoreAsync(entity);
            productCategories.Add(productCategory);
        }

        return productCategories;
    }

    /// <summary>
    /// 指定したカテゴリ名が既に存在するか確認する。
    /// </summary>
    /// <param name="name">カテゴリ名。</param>
    /// <returns>存在する場合true。</returns>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.ProductCategories
            .AnyAsync(productCategory => productCategory.Name == name);
    }

    /// <summary>
    /// 商品カテゴリを登録する。
    /// </summary>
    /// <param name="productCategory">商品カテゴリ。</param>
    public async Task CreateAsync(ProductCategory productCategory)
    {
        var entity = await _adapter.ConvertAsync(productCategory);

        await _context.ProductCategories.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}