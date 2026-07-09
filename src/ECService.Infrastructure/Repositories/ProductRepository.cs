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
            // ① 削除されていない商品を取得する
            // 商品カテゴリは ProductEntity から辿れるので Include する
            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p => p.DeleteFlag == 0)
                .ToListAsync();

            var products = new List<Product>();

            // ② 商品ごとに在庫を取得して、FactoryでProductに復元する
            foreach (var productEntity in productEntities)
            {
                var stockEntity = await _context.ProductStocks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

                if (stockEntity is null)
                {
                    continue;
                }

                var product = await _factory.Factory(
                    productEntity,
                    stockEntity,
                    productEntity.ProductCategory
                );

                products.Add(product);
            }

            return products;
        }
        catch (Exception ex)
        {
            throw new InternalException("商品一覧取得中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task<List<Product>> SelectByCategoryAsync(string categoryUuid)
    {
        try
        {
            // ① 削除されていない商品かつ、指定カテゴリの商品だけ取得する
            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p =>
                    p.DeleteFlag == 0 &&
                    p.ProductCategory.CategoryUuid == categoryUuid)
                .ToListAsync();

            var products = new List<Product>();

            // ② 商品ごとに在庫を取得する
            foreach (var productEntity in productEntities)
            {
                var stockEntity = await _context.ProductStocks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

                if (stockEntity is null)
                {
                    continue;
                }

                // ③ EntityたちからProductを復元する
                var product = await _factory.Factory(
                    productEntity,
                    stockEntity,
                    productEntity.ProductCategory
                );

                products.Add(product);
            }

            return products;
        }
        catch (Exception ex)
        {
            throw new InternalException($"カテゴリUUID:{categoryUuid}の商品取得中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task<Product?> SelectByUuidAsync(string productUuid)
    {
        try
        {
            // ① 商品UUIDで商品を1件取得する
            var productEntity = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == productUuid &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return null;
            }

            // ② 商品IDで在庫を取得する
            var stockEntity = await _context.ProductStocks
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

            if (stockEntity is null)
            {
                return null;
            }

            // ③ EntityたちからProductを復元する
            return await _factory.Factory(
                productEntity,
                stockEntity,
                productEntity.ProductCategory
            );
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{productUuid}の商品取得中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        try
        {
            // ① 削除されていない商品の中に、
            // 同じ商品名が存在するか確認する
            return await _context.Products
                .AsNoTracking()
                .AnyAsync(p =>
                    p.Name == name &&
                    p.DeleteFlag == 0);
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品名:{name}の商品存在確認中に予期しないエラーが発生しました。", ex);
        }
    }

    public async Task CreateAsync(Product product)
    {
        try
        {
            // ① ProductをProductEntityへ変換する
            var productEntity = await _productEntityAdapter.ConvertAsync(product);

            // ② カテゴリUUIDからカテゴリEntityを取得する
            var categoryEntity = await _context.ProductCategories
                .SingleOrDefaultAsync(c =>
                    c.CategoryUuid == Guid.Parse(product.ProductCategory.CategoryUuid));

            if (categoryEntity is null)
            {
                throw new InternalException("指定された商品カテゴリが存在しません。");
            }

            // ③ 商品EntityへカテゴリIDを設定する
            productEntity.ProductCategoryId = categoryEntity.Id;

            // ④ 商品を登録する
            await _context.Products.AddAsync(productEntity);

            // ⑤ SaveChangesして商品IDを採番する
            await _context.SaveChangesAsync();

            // ⑥ 商品在庫Entityを作成する
            var stockEntity = await _productStockAdapter.ConvertAsync(product.ProductStock);

            // ⑦ 商品ID(FK)を設定する
            stockEntity.ProductId = productEntity.Id;

            // ⑧ 在庫を登録する
            await _context.ProductStocks.AddAsync(stockEntity);

            // ⑨ DBへ保存する
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InternalException("商品登録中に予期しないエラーが発生しました。", ex);
        }
    }
    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            retu await _context.Products
                .Include(p => p.ProductCategory)
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

    public async Task<bool> DeleteAsync(string productUuid)
    {
        try
        {
            var entity = await _context.Products
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == productUuid &&
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
            throw new InternalException($"商品UUID:{productUuid}の商品削除中に予期しないエラーが発生しました。", ex);
        }
    }
}