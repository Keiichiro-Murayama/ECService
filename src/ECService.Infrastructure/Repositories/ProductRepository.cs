using ECService.Infrastructure.Exceptions;
using ECService.Domain.Models;
// using ECService.Domain.Exceptions;//石原:exception2個あるから変かも
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
    /// すべての商品を取得する
    /// </summary>
    /// <returns>商品一覧</returns>
    public async Task<List<Product>> SelectAllAsync()
    {
        try
        {
            // 削除されていない商品を取得する
            // 商品カテゴリも使用するため Include する
            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p => p.DeleteFlag == 0)
                .ToListAsync();

            var products = new List<Product>();

            // 商品ごとに在庫を取得して、FactoryでProductに復元する
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

    /// <summary>
    /// カテゴリで商品一覧を取得する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <returns>指定カテゴリの商品一覧</returns>
    public async Task<List<Product>> SelectByCategoryAsync(string categoryUuid)
    {
        try
        {
            // 削除されていない商品かつ、指定カテゴリの商品だけ取得する
            var productEntities = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p =>
                    p.DeleteFlag == 0 &&
                    p.ProductCategory.CategoryUuid == Guid.Parse(categoryUuid))
                .ToListAsync();

            var products = new List<Product>();

            // 商品ごとに在庫を取得して、FactoryでProductに復元する
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
            throw new InternalException($"カテゴリUUID:{categoryUuid}の商品取得中に予期しないエラーが発生しました。", ex);
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
            // 商品UUIDで商品を1件取得する
            var productEntity = await _context.Products
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == Guid.Parse(productUuid) &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return null;
            }

            // 商品IDで在庫を取得する
            var stockEntity = await _context.ProductStocks
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

            if (stockEntity is null)
            {
                return null;
            }

            // EntityたちからProductを復元する
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

    /// <summary>
    /// 指定された商品名の存在有無を返す
    /// </summary>
    /// <param name="name">商品名</param>
    /// <returns>true:存在する false:存在しない</returns>
    public async Task<bool> ExistsByNameAsync(string name)
    {
        try
        {
            // 削除されていない商品の中に、同じ商品名が存在するか確認する
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

    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    public async Task CreateAsync(Product product)
    {
        try
        {
            // ProductをProductEntityへ変換する
            var productEntity = await _productEntityAdapter.ConvertAsync(product);

            // カテゴリUUIDからカテゴリEntityを取得する
            var categoryEntity = await _context.ProductCategories
                .SingleOrDefaultAsync(c =>
                    c.CategoryUuid == Guid.Parse(product.ProductCategory.CategoryUuid));

            if (categoryEntity is null)
            {
                throw new InternalException("指定された商品カテゴリが存在しません。");
            }

            // 商品EntityへカテゴリIDを設定する
            productEntity.ProductCategoryId = categoryEntity.Id;

            // 商品を登録する
            await _context.Products.AddAsync(productEntity);

            // SaveChangesして商品IDを採番する
            await _context.SaveChangesAsync();

            // 商品在庫Entityを作成する
            var stockEntity = await _productStockAdapter.ConvertAsync(product.ProductStock);

            // 商品IDを設定する
            stockEntity.ProductId = productEntity.Id;

            // 在庫を登録する
            await _context.ProductStocks.AddAsync(stockEntity);

            // DBへ保存する
            await _context.SaveChangesAsync();
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
            // 更新対象の商品を取得する
            var productEntity = await _context.Products
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == Guid.Parse(product.ProductUuid) &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return false;
            }

            // カテゴリを取得する
            var categoryEntity = await _context.ProductCategories
                .SingleOrDefaultAsync(c =>
                    c.CategoryUuid == Guid.Parse(product.ProductCategory.CategoryUuid));

            if (categoryEntity is null)
            {
                throw new InternalException("指定された商品カテゴリが存在しません。");
            }

            // 商品情報を更新する
            productEntity.Name = product.Name;
            productEntity.Price = product.Price;
            productEntity.ImageUrl = product.ImageUrl;
            productEntity.ProductCategoryId = categoryEntity.Id;

            // 在庫情報を取得する
            var stockEntity = await _context.ProductStocks
                .SingleOrDefaultAsync(s => s.ProductId == productEntity.Id);

            if (stockEntity is null)
            {
                return false;
            }

            // 在庫数を更新する
            stockEntity.Quantity = product.ProductStock.Quantity;

            // DBに保存する
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{product.ProductUuid}の商品更新中に予期しないエラーが発生しました。", ex);
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
            // 削除対象の商品を取得する
            var productEntity = await _context.Products
                .SingleOrDefaultAsync(p =>
                    p.ProductUuid == Guid.Parse(productUuid) &&
                    p.DeleteFlag == 0);

            if (productEntity is null)
            {
                return false;
            }

            // 物理削除ではなく論理削除する
            productEntity.DeleteFlag = 1;

            // DBへ反映する
            await _context.SaveChangesAsync();

            return true;
        }
        catch (Exception ex)
        {
            throw new InternalException($"商品UUID:{productUuid}の商品削除中に予期しないエラーが発生しました。", ex);
        }
    }
}