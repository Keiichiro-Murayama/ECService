using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Exceptions;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

[TestClass]
public class ProductRepositoryTests
{
    private AppDbContext _context = null!;
    private ProductRepository _repository = null!;

    // 各テストで作成したデータを削除するために保持する
    private readonly List<Guid> _productUuids = [];
    private readonly List<Guid> _categoryUuids = [];

    [TestInitialize]
    public async Task Setup()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("TEST_DB_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "環境変数 TEST_DB_CONNECTION_STRING が設定されていません。");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new AppDbContext(options);

        if (!await _context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用PostgreSQLへ接続できません。");
        }

        var categoryAdapter =
            new ProductCategoryEntityAdapter();

        var stockAdapter =
            new ProductStockEntityAdapter();

        var productFactory =
            new ProductFactory(
                categoryAdapter,
                stockAdapter);

        var productEntityAdapter =
            new ProductEntityAdapter();

        _repository = new ProductRepository(
            _context,
            productFactory,
            productEntityAdapter,
            stockAdapter);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        // 追跡済みEntityの状態を一度破棄する
        _context.ChangeTracker.Clear();

        if (_productUuids.Count > 0)
        {
            var products = await _context.Products
                .Where(product =>
                    _productUuids.Contains(product.ProductUuid))
                .ToListAsync();

            var productIds = products
                .Select(product => product.Id)
                .ToList();

            if (productIds.Count > 0)
            {
                var stocks = await _context.ProductStocks
                    .Where(stock =>
                        productIds.Contains(stock.ProductId))
                    .ToListAsync();

                _context.ProductStocks.RemoveRange(stocks);
            }

            _context.Products.RemoveRange(products);
        }

        if (_categoryUuids.Count > 0)
        {
            var categories = await _context.ProductCategories
                .Where(category =>
                    _categoryUuids.Contains(category.CategoryUuid))
                .ToListAsync();

            _context.ProductCategories.RemoveRange(categories);
        }

        await _context.SaveChangesAsync();
        await _context.DisposeAsync();
    }

    // =========================================================
    // SelectAllAsync
    // =========================================================

    [TestMethod(DisplayName = "削除されていない商品一覧を取得できる")]
    public async Task SelectAllAsync_ShouldReturnActiveProducts()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("一覧テストカテゴリ");

        var product =
            await InsertProductAsync(
                category,
                "一覧テスト商品",
                1000,
                10);

        // Act
        var result = await _repository.SelectAllAsync();

        // Assert
        var actual = result.SingleOrDefault(
            item => item.ProductUuid ==
                    product.ProductUuid.ToString());

        Assert.IsNotNull(actual);
        Assert.AreEqual("一覧テスト商品", actual.Name);
        Assert.AreEqual(1000, actual.Price);
        Assert.AreEqual(10, actual.ProductStock.Quantity);
        Assert.AreEqual(
            category.CategoryUuid.ToString(),
            actual.ProductCategory.CategoryUuid);
    }

    [TestMethod(DisplayName = "論理削除済みの商品は一覧に含まれない")]
    public async Task SelectAllAsync_ShouldNotReturnDeletedProduct()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("削除済み一覧カテゴリ");

        var product =
            await InsertProductAsync(
                category,
                "削除済み商品",
                1000,
                10,
                deleteFlag: 1);

        // Act
        var result = await _repository.SelectAllAsync();

        // Assert
        Assert.IsFalse(
            result.Any(item =>
                item.ProductUuid ==
                product.ProductUuid.ToString()));
    }

    [TestMethod(DisplayName = "在庫が存在しない商品は一覧に含まれない")]
    public async Task SelectAllAsync_ShouldSkipProduct_WhenStockDoesNotExist()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("在庫なしカテゴリ");

        var productUuid = Guid.NewGuid();
        _productUuids.Add(productUuid);

        var product = new ProductEntity
        {
            ProductUuid = productUuid,
            ProductCategoryId = category.Id,
            Name = "在庫なし商品",
            Price = 1000,
            ImageUrl = "https://example.com/no-stock.png",
            DeleteFlag = 0
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SelectAllAsync();

        // Assert
        Assert.IsFalse(
            result.Any(item =>
                item.ProductUuid ==
                productUuid.ToString()));
    }

    // =========================================================
    // SelectByCategoryAsync
    // =========================================================

    [TestMethod(DisplayName = "カテゴリUUIDで商品一覧を取得できる")]
    public async Task SelectByCategoryAsync_ShouldReturnProductsInCategory()
    {
        // Arrange
        var targetCategory =
            await InsertCategoryAsync("検索対象カテゴリ");

        var otherCategory =
            await InsertCategoryAsync("別カテゴリ");

        var targetProduct =
            await InsertProductAsync(
                targetCategory,
                "対象カテゴリ商品",
                1500,
                20);

        await InsertProductAsync(
            otherCategory,
            "別カテゴリ商品",
            2000,
            30);

        // Act
        var result = await _repository.SelectByCategoryAsync(
            targetCategory.CategoryUuid.ToString());

        // Assert
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(
            targetProduct.ProductUuid.ToString(),
            result[0].ProductUuid);
        Assert.AreEqual("対象カテゴリ商品", result[0].Name);
    }

    [TestMethod(DisplayName = "指定カテゴリに商品がない場合は空の一覧を返す")]
    public async Task SelectByCategoryAsync_ShouldReturnEmptyList_WhenNoProductExists()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("商品なしカテゴリ");

        // Act
        var result = await _repository.SelectByCategoryAsync(
            category.CategoryUuid.ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod(DisplayName = "カテゴリUUID形式が不正な場合は例外を送出する")]
    public async Task SelectByCategoryAsync_ShouldThrow_WhenUuidIsInvalid()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _repository.SelectByCategoryAsync(
                    "invalid-category-uuid"));

        // Assert
        Assert.AreEqual(
            "カテゴリUUIDの形式が不正です。",
            exception.Message);
    }

    // =========================================================
    // SelectByUuidAsync
    // =========================================================

    [TestMethod(DisplayName = "商品UUIDで商品を取得できる")]
    public async Task SelectByUuidAsync_ShouldReturnProduct()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("UUID検索カテゴリ");

        var product =
            await InsertProductAsync(
                category,
                "UUID検索商品",
                2500,
                15);

        // Act
        var result = await _repository.SelectByUuidAsync(
            product.ProductUuid.ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            product.ProductUuid.ToString(),
            result.ProductUuid);
        Assert.AreEqual("UUID検索商品", result.Name);
        Assert.AreEqual(2500, result.Price);
        Assert.AreEqual(15, result.ProductStock.Quantity);
    }

    [TestMethod(DisplayName = "商品が存在しない場合はnullを返す")]
    public async Task SelectByUuidAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Act
        var result = await _repository.SelectByUuidAsync(
            Guid.NewGuid().ToString());

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod(DisplayName = "商品UUID形式が不正な場合は例外を送出する")]
    public async Task SelectByUuidAsync_ShouldThrow_WhenUuidIsInvalid()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _repository.SelectByUuidAsync(
                    "invalid-product-uuid"));

        // Assert
        Assert.AreEqual(
            "商品UUIDの形式が不正です。",
            exception.Message);
    }

    // =========================================================
    // ExistsByNameAsync
    // =========================================================

    [TestMethod(DisplayName = "商品名が存在する場合はtrueを返す")]
    public async Task ExistsByNameAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("存在確認カテゴリ");

        await InsertProductAsync(
            category,
            "存在確認商品",
            1000,
            10);

        // Act
        var result =
            await _repository.ExistsByNameAsync("存在確認商品");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "商品名が存在しない場合はfalseを返す")]
    public async Task ExistsByNameAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Act
        var result =
            await _repository.ExistsByNameAsync(
                $"不存在商品{Guid.NewGuid():N}");

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "論理削除済みの商品名はfalseを返す")]
    public async Task ExistsByNameAsync_ShouldReturnFalse_WhenProductIsDeleted()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("削除済み存在確認");

        await InsertProductAsync(
            category,
            "削除済み確認商品",
            1000,
            10,
            deleteFlag: 1);

        // Act
        var result =
            await _repository.ExistsByNameAsync(
                "削除済み確認商品");

        // Assert
        Assert.IsFalse(result);
    }

    // =========================================================
    // CreateAsync
    // =========================================================

    [TestMethod(DisplayName = "商品と在庫を登録できる")]
    public async Task CreateAsync_ShouldCreateProductAndStock()
    {
        // Arrange
        var categoryEntity =
            await InsertCategoryAsync("登録テストカテゴリ");

        var category = new ProductCategory(
            categoryEntity.CategoryUuid.ToString(),
            categoryEntity.Name);

        var stock = ProductStock.Create(25);

        var product = Product.Create(
            "登録テスト商品",
            3000,
            "https://example.com/create.png",
            category,
            stock);

        _productUuids.Add(Guid.Parse(product.ProductUuid));

        // Act
        await _repository.CreateAsync(product);

        // Assert
        var savedProduct = await _context.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(entity =>
                entity.ProductUuid ==
                Guid.Parse(product.ProductUuid));

        Assert.IsNotNull(savedProduct);
        Assert.AreEqual("登録テスト商品", savedProduct.Name);
        Assert.AreEqual(3000, savedProduct.Price);
        Assert.AreEqual(categoryEntity.Id,
                        savedProduct.ProductCategoryId);

        var savedStock = await _context.ProductStocks
            .AsNoTracking()
            .SingleOrDefaultAsync(entity =>
                entity.ProductId == savedProduct.Id);

        Assert.IsNotNull(savedStock);
        Assert.AreEqual(25, savedStock.Quantity);
        Assert.AreEqual(
            Guid.Parse(stock.StockUuid),
            savedStock.StockUuid);
    }

    [TestMethod(DisplayName = "存在しないカテゴリを指定した場合は例外を送出する")]
    public async Task CreateAsync_ShouldThrow_WhenCategoryDoesNotExist()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid().ToString(),
            "存在しないカテゴリ");

        var stock = ProductStock.Create(10);

        var product = Product.Create(
            "登録失敗商品",
            1000,
            "https://example.com/not-found.png",
            category,
            stock);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _repository.CreateAsync(product));

        // Assert
        Assert.AreEqual(
            "指定された商品カテゴリが存在しません。",
            exception.Message);
    }

    // =========================================================
    // UpdateAsync
    // =========================================================

    [TestMethod(DisplayName = "商品情報と在庫を更新できる")]
    public async Task UpdateAsync_ShouldUpdateProductAndStock()
    {
        // Arrange
        var originalCategory =
            await InsertCategoryAsync("更新前カテゴリ");

        var newCategory =
            await InsertCategoryAsync("更新後カテゴリ");

        var savedProduct =
            await InsertProductAsync(
                originalCategory,
                "更新前商品",
                1000,
                10);

        var domainCategory = new ProductCategory(
            newCategory.CategoryUuid.ToString(),
            newCategory.Name);

        var domainStock = ProductStock.Create(50);

        var updatedProduct = Product.Restore(
            savedProduct.ProductUuid.ToString(),
            "更新後商品",
            5000,
            "https://example.com/updated.png",
            domainCategory,
            0,
            domainStock);

        // Act
        var result =
            await _repository.UpdateAsync(updatedProduct);

        // Assert
        Assert.IsTrue(result);

        _context.ChangeTracker.Clear();

        var actualProduct = await _context.Products
            .AsNoTracking()
            .SingleAsync(entity =>
                entity.ProductUuid ==
                savedProduct.ProductUuid);

        Assert.AreEqual("更新後商品", actualProduct.Name);
        Assert.AreEqual(5000, actualProduct.Price);
        Assert.AreEqual(
            "https://example.com/updated.png",
            actualProduct.ImageUrl);
        Assert.AreEqual(
            newCategory.Id,
            actualProduct.ProductCategoryId);

        var actualStock = await _context.ProductStocks
            .AsNoTracking()
            .SingleAsync(entity =>
                entity.ProductId == actualProduct.Id);

        Assert.AreEqual(50, actualStock.Quantity);
    }

    [TestMethod(DisplayName = "更新対象の商品が存在しない場合はfalseを返す")]
    public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        var categoryEntity =
            await InsertCategoryAsync("更新不存在カテゴリ");

        var category = new ProductCategory(
            categoryEntity.CategoryUuid.ToString(),
            categoryEntity.Name);

        var product = Product.Restore(
            Guid.NewGuid().ToString(),
            "存在しない商品",
            1000,
            "https://example.com/not-exist.png",
            category,
            0,
            ProductStock.Create(10));

        // Act
        var result =
            await _repository.UpdateAsync(product);

        // Assert
        Assert.IsFalse(result);
    }

    // =========================================================
    // DeleteAsync
    // =========================================================

    [TestMethod(DisplayName = "商品を論理削除できる")]
    public async Task DeleteAsync_ShouldSoftDeleteProduct()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("削除テストカテゴリ");

        var product =
            await InsertProductAsync(
                category,
                "削除テスト商品",
                1000,
                10);

        // Act
        var result = await _repository.DeleteAsync(
            product.ProductUuid.ToString());

        // Assert
        Assert.IsTrue(result);

        _context.ChangeTracker.Clear();

        var actual = await _context.Products
            .AsNoTracking()
            .SingleAsync(entity =>
                entity.ProductUuid ==
                product.ProductUuid);

        Assert.AreEqual(1, actual.DeleteFlag);
    }

    [TestMethod(DisplayName = "存在しない商品を削除する場合はfalseを返す")]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Act
        var result = await _repository.DeleteAsync(
            Guid.NewGuid().ToString());

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "削除対象UUIDの形式が不正な場合は例外を送出する")]
    public async Task DeleteAsync_ShouldThrow_WhenUuidIsInvalid()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _repository.DeleteAsync(
                    "invalid-product-uuid"));

        // Assert
        Assert.AreEqual(
            "商品UUIDの形式が不正です。",
            exception.Message);
    }

    // =========================================================
    // テストデータ作成用メソッド
    // =========================================================

    private async Task<ProductCategoryEntity> InsertCategoryAsync(
        string name)
    {
        var category = new ProductCategoryEntity
        {
            CategoryUuid = Guid.NewGuid(),
            Name = name
        };

        _categoryUuids.Add(category.CategoryUuid);

        await _context.ProductCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        return category;
    }

    private async Task<ProductEntity> InsertProductAsync(
        ProductCategoryEntity category,
        string name,
        int price,
        int quantity,
        int deleteFlag = 0)
    {
        var product = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            ProductCategoryId = category.Id,
            Name = name,
            Price = price,
            ImageUrl = "https://example.com/test.png",
            DeleteFlag = deleteFlag
        };

        _productUuids.Add(product.ProductUuid);

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var stock = new ProductStockEntity
        {
            StockUuid = Guid.NewGuid(),
            ProductId = product.Id,
            Quantity = quantity
        };

        await _context.ProductStocks.AddAsync(stock);
        await _context.SaveChangesAsync();

        return product;
    }
}