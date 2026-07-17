using ECService.Infrastructure.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 商品Repository
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly ProductFactory _factory;
    private readonly ProductStockEntityAdapter _productStockAdapter;
    private readonly ProductEntityAdapter _productEntityAdapter;

    public ProductRepository(
        AppDbContext context,
        ProductFactory factory,
        ProductEntityAdapter productEntityAdapter,
        ProductStockEntityAdapter productStockAdapter)
    {
        _context = context;
        _factory = factory;
        _productEntityAdapter = productEntityAdapter;
        _productStockAdapter = productStockAdapter;
    }

    /// <summary>
    /// UUID文字列をGuidへ変換する。
    /// </summary>
    private static Guid ConvertToGuid(string uuid, string errorMessage)
    {
        if (!Guid.TryParse(uuid, out var parsedUuid))
        {
            throw new InternalException(errorMessage);
        }

        return parsedUuid;
    }

    /// <summary>
    /// すべての商品を取得する
    /// </summary>
    /// <returns>商品一覧</returns>
    public async Task<List<Product>> SelectAllAsync()
    {
        try
        {
            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p => p.DeleteFlag == 0)
                .ToListAsync();

            var products = new List<Product>();

            foreach (var productEntity in productEntities)
            {
                var stockEntity = await _context.ProductStocks
                    .AsNoTracking()
                    .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

                if (stockEntity is null)
                {
                    throw new InternalException("商品在庫が存在しません。");
                }

                var product = await _factory.Factory(
                    productEntity,
                    stockEntity,
                    productEntity.ProductCategory);

                products.Add(product);
            }

            return products;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException("商品一覧取得中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// カテゴリで商品一覧を取得する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <returns>指定カテゴリの商品一覧</returns>
    public async Task<List<Product>> SelectByCategoryAsync(string categoryUuid)
    {
        try
        {
            var parsedCategoryUuid = ConvertToGuid(
                categoryUuid,
                "カテゴリUUIDの形式が不正です。");

            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p =>
                    p.DeleteFlag == 0 &&
                    p.ProductCategory.CategoryUuid == parsedCategoryUuid)
                .ToListAsync();

            var products = new List<Product>();

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
                    productEntity.ProductCategory);

                products.Add(product);
            }

            return products;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"カテゴリUUID:{categoryUuid}の商品取得中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 指定された商品UUIDの商品と在庫、商品カテゴリを返す
    /// </summary>
    /// <param name="productUuid">商品UUID</param>
    /// <returns>Product または null</returns>
    public async Task<Product?> SelectByUuidAsync(string productUuid)
    {
        try
        {
            var parsedProductUuid = ConvertToGuid(
                productUuid,
                "商品UUIDの形式が不正です。");

            var productEntity = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == parsedProductUuid &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return null;
            }

            var stockEntity = await _context.ProductStocks
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

            if (stockEntity is null)
            {
                return null;
            }

            return await _factory.Factory(
                productEntity,
                stockEntity,
                productEntity.ProductCategory);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"商品UUID:{productUuid}の商品取得中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 指定された商品名の存在有無を返す
    /// </summary>
    /// <param name="name">商品名</param>
    /// <returns>true:存在する false:存在しない</returns>
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
            throw new InternalException(
                $"商品名:{name}の商品存在確認中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    public async Task CreateAsync(Product product)
    {
        try
        {
            var productEntity = await _productEntityAdapter.ConvertAsync(product);

            var parsedCategoryUuid = ConvertToGuid(
                product.ProductCategory.CategoryUuid,
                "商品カテゴリUUIDの形式が不正です。");

            var categoryEntity = await _context.ProductCategories
                .SingleOrDefaultAsync(c =>
                    c.CategoryUuid == parsedCategoryUuid);

            if (categoryEntity is null)
            {
                throw new InternalException("指定された商品カテゴリが存在しません。");
            }

            productEntity.ProductCategoryId = categoryEntity.Id;

            await _context.Products.AddAsync(productEntity);
            await _context.SaveChangesAsync();

            var stockEntity = await _productStockAdapter.ConvertAsync(product.ProductStock);

            stockEntity.ProductId = productEntity.Id;

            await _context.ProductStocks.AddAsync(stockEntity);
            await _context.SaveChangesAsync();
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException("商品登録中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 商品を更新する
    /// </summary>
    /// <param name="product">更新対象の商品</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            var parsedProductUuid = ConvertToGuid(
                product.ProductUuid,
                "商品UUIDの形式が不正です。");

            var productEntity = await _context.Products
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == parsedProductUuid &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return false;
            }

            var parsedCategoryUuid = ConvertToGuid(
                product.ProductCategory.CategoryUuid,
                "商品カテゴリUUIDの形式が不正です。");

            var categoryEntity = await _context.ProductCategories
                .SingleOrDefaultAsync(c =>
                    c.CategoryUuid == parsedCategoryUuid);

            if (categoryEntity is null)
            {
                throw new InternalException("指定された商品カテゴリが存在しません。");
            }

            productEntity.Name = product.Name;
            productEntity.Price = product.Price;
            productEntity.ImageUrl = product.ImageUrl;
            productEntity.ProductCategoryId = categoryEntity.Id;

            var stockEntity = await _context.ProductStocks
                .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

            if (stockEntity is null)
            {
                return false;
            }

            stockEntity.Quantity = product.ProductStock.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"商品UUID:{product.ProductUuid}の商品更新中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 商品を削除する
    /// </summary>
    /// <param name="productUuid">削除対象の商品UUID</param>
    /// <returns>true:削除成功 false:削除失敗</returns>
    public async Task<bool> DeleteAsync(string productUuid)
    {
        try
        {
            var parsedProductUuid = ConvertToGuid(
                productUuid,
                "商品UUIDの形式が不正です。");

            var updatedCount = await _context.Products
                .Where(p =>
                    p.ProductUuid == parsedProductUuid &&
                    p.DeleteFlag == 0)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.DeleteFlag, 1));

            return updatedCount > 0;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"商品UUID:{productUuid}の商品削除中に予期しないエラーが発生しました。",
                ex);
        }
    }
}