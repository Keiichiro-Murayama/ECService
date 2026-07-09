using ECService.Infrastructure.Exceptions;
using ECService.Domain.Models;
using ECService.Domains.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 商品Repository
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly ProductFactory _factory;

    public ProductRepository(AppDbContext context, ProductFactory factory)
    {
        _context = context;
        _factory = factory;
    }

    /// <summary>
    /// すべての商品を取得する
    /// </summary>
    /// <returns>商品一覧</returns>
    public async Task<List<Product>> SelectAllAsync()
    {
        try
        {
            // 商品テーブルから、削除されていない商品だけ取得する
            // ProductCategory は商品カテゴリ名などを使うために一緒に取得する
            var entities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p => p.DeleteFlag == 0)
                .ToListAsync();
            //  ProductEntity のリストを Domain の Product リストに変換する
            return await _factory.RestoreAsync(entities);
        }
            catch (Exception ex)
            {
                throw new InternalException("商品一覧取得中に予期しないエラーが発生しました。", ex.Message);
            }
    }

    public async Task<List<Product>> SelectByCategoryAsync(string categoryUuid)
    {
        try
        {
            var entities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p =>
                    p.DeleteFlag == 0 &&
                    p.ProductCategory!.CategoryUuid == categoryUuid)
                .ToListAsync();

            return await _factory.RestoreAsync(entities);
        }
        catch (Exception ex)
        {
            throw new InternalException($"カテゴリUUID:{categoryUuid}の商品検索中に予期しないエラーが発生しました。", ex.Message);
        }
    }

    public async Task<Product?> SelectByUuidAsync(string productUuid)
    {
        try
        {
            var entity = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == productUuid &&
                    p.DeleteFlag == 0);

            if (entity is null)
            {
                return null;
            }

            return await _factory.RestoreAsync(ProductEntity, ProductStockEntity);
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{productUuid}の商品取得中に予期しないエラーが発生しました。", ex.Message);
        }
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        try
        {
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p =>
                    p.Name == name &&
                    p.DeleteFlag == 0);
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品名:{name}の商品存在確認中に予期しないエラーが発生しました。", ex.Message);
        }
    }

    public async Task CreateAsync(Product product)
    {
        try
        {
            var category = await _context.ProductCategories
                .SingleOrDefaultAsync(c => c.CategoryUuid == product.ProductCategory!.CategoryUuid);

            if (category is null)
            {
                throw new DomainException("指定された商品カテゴリが存在しません。");
            }

            var entity = await _factory.ConvertAsync(product);

            entity.ProductCategory = null;
            entity.ProductCategoryId = category.Id;
            entity.DeleteFlag = 0;

            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException("商品登録中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task UpdateAsync(Product product)
    {
        try
        {
            var entity = await _context.Products
                .Include(p => p.ProductCategory)
                .Include(p => p.ProductStock)
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == product.ProductUuid &&
                    p.DeleteFlag == 0);

            if (entity is null)
            {
                throw new DomainException("更新対象の商品が存在しません。");
            }

            var category = await _context.ProductCategories
                .SingleOrDefaultAsync(c => c.CategoryUuid == product.ProductCategory!.CategoryUuid);

            if (category is null)
            {
                throw new DomainException("指定された商品カテゴリが存在しません。");
            }

            entity.Name = product.Name;
            entity.Price = product.Price;
            entity.ProductCategoryId = category.Id;
            entity.ImageUrl = product.ImageUrl;

            if (entity.ProductStock is not null && product.ProductStock is not null)
            {
                entity.ProductStock.Quantity = product.ProductStock.Quantity;
            }

            await _context.SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{product.ProductUuid}の商品更新中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task DeleteAsync(Product product)
    {
        try
        {
            var entity = await _context.Products
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == product.ProductUuid &&
                    p.DeleteFlag == 0);

            if (entity is null)
            {
                throw new DomainException("削除対象の商品が存在しません。");
            }

            // 論理削除
            entity.DeleteFlag = 1;

            await _context.SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{product.ProductUuid}の商品削除中に予期しないエラーが発生しました。", ex);
        }
    }
}