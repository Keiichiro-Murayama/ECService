using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Exceptions;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

/// <summary>
/// ProductRepositoryの単体テスト
/// </summary>
[TestClass]
[TestCategory("Repositories")]
[DoNotParallelize]
public class ProductRepositoryTests
{
    private AppDbContext _dbContext = null!;
    private IProductRepository _productRepository = null!;

    private readonly List<Guid> _categoryUuids = new();
    private readonly List<Guid> _productUuids = new();
    private readonly List<Guid> _stockUuids = new();

    [TestInitialize]
    public async Task TestInit()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettingsTests.json",
                optional: false,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration
            .GetConnectionString("ECServiceDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "appsettingsTests.json に ConnectionStrings:ECServiceDB が設定されていません。");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        if (!await _dbContext.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用PostgreSQLへ接続できません。");
        }

        var categoryAdapter = new ProductCategoryEntityAdapter();
        var stockAdapter = new ProductStockEntityAdapter();
        var factory = new ProductFactory(categoryAdapter, stockAdapter);
        var productEntityAdapter = new ProductEntityAdapter();

        _productRepository = new ProductRepository(
            _dbContext,
            factory,
            productEntityAdapter,
            stockAdapter);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_dbContext is null)
        {
            return;
        }

        _dbContext.ChangeTracker.Clear();

        if (_stockUuids.Count > 0)
        {
            var stocks = await _dbContext.ProductStocks
                .Where(stock => _stockUuids.Contains(stock.StockUuid))
                .ToListAsync();

            _dbContext.ProductStocks.RemoveRange(stocks);
            await _dbContext.SaveChangesAsync();
        }

        if (_productUuids.Count > 0)
        {
            var products = await _dbContext.Products
                .Where(product => _productUuids.Contains(product.ProductUuid))
                .ToListAsync();

            _dbContext.Products.RemoveRange(products);
            await _dbContext.SaveChangesAsync();
        }

        if (_categoryUuids.Count > 0)
        {
            var categories = await _dbContext.ProductCategories
                .Where(category =>
                    _categoryUuids.Contains(category.CategoryUuid))
                .ToListAsync();

            _dbContext.ProductCategories.RemoveRange(categories);
            await _dbContext.SaveChangesAsync();
        }

        await _dbContext.DisposeAsync();
    }

    [TestMethod(DisplayName = "削除されていない商品一覧を取得できる")]
    public async Task SelectAllAsync_ShouldReturnAllActiveProducts()
    {
        // Arrange
        var category =
            await InsertCategoryAsync("商品一覧テストカテゴリ");

        var createdProductUuids = new List<string>();

        for (var i = 1; i <= 6; i++)
        {
            var product = await InsertProductAsync(
                category,
                $"一覧テスト商品{i}",
                i * 100,
                deleteFlag: 0);

            await InsertStockAsync(
                product,
                i * 10);

            createdProductUuids.Add(
                product.ProductUuid.ToString());
        }

        // Act
        var products =
            await _productRepository.SelectAllAsync();

        // Assert
        Assert.IsNotNull(products);

        var createdProducts = products
            .Where(product =>
                createdProductUuids.Contains(product.ProductUuid))
            .ToList();

        Assert.HasCount(6, createdProducts);

        for (var i = 1; i <= 6; i++)
        {
            var actual = createdProducts.Single(product =>
                product.Name.StartsWith($"一覧テスト商品{i}"));

            Assert.AreEqual(i * 100, actual.Price);
            Assert.AreEqual(0, actual.DeleteFlg);
        }
    }
    [TestMethod(DisplayName = "削除済み商品は商品一覧に含まれない")]
    public async Task SelectAllAsync_WhenDeleteFlagIsOne_ShouldNotReturnProduct()
    {
        // Arrange
        var category = await InsertCategoryAsync("削除済カテゴリ");
        var deletedProduct = await InsertProductAsync(
            category,
            "削除済商品",
            1000,
            deleteFlag: 1);

        await InsertStockAsync(deletedProduct, 10);

        // Act
        var products = await _productRepository.SelectAllAsync();

        // Assert
        Assert.IsNotNull(products);
        Assert.IsFalse(products.Any(product =>
            product.ProductUuid == deletedProduct.ProductUuid.ToString()));
    }

    [TestMethod(DisplayName = "指定カテゴリの商品一覧を取得できる")]
    public async Task SelectByCategoryAsync_WhenCategoryExists_ShouldReturnProducts()
    {
        // Arrange
        var targetCategory = await InsertCategoryAsync("文具");

        var product1 = await InsertProductAsync(
            targetCategory,
            "高級ボールペン",
            1200);
        await InsertStockAsync(product1, 50);

        var product2 = await InsertProductAsync(
            targetCategory,
            "耐水ノートA5",
            450);
        await InsertStockAsync(product2, 120);

        var otherCategory = await InsertCategoryAsync("雑貨");
        var otherProduct = await InsertProductAsync(
            otherCategory,
            "別カテゴリ商品",
            880);
        await InsertStockAsync(otherProduct, 20);

        var categoryUuid = targetCategory.CategoryUuid.ToString();

        // Act
        var products = await _productRepository
            .SelectByCategoryAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(products);
        Assert.HasCount(2, products);
        Assert.IsTrue(products.All(product =>
            product.ProductCategory.CategoryUuid == categoryUuid));

        var actualProduct1 = products.Single(product =>
            product.ProductUuid == product1.ProductUuid.ToString());

        Assert.AreEqual(product1.Name, actualProduct1.Name);
        Assert.AreEqual(product1.Price, actualProduct1.Price);
        Assert.AreEqual(targetCategory.Name, actualProduct1.ProductCategory.Name);
        Assert.AreEqual(50, actualProduct1.ProductStock.Quantity);

        var actualProduct2 = products.Single(product =>
            product.ProductUuid == product2.ProductUuid.ToString());

        Assert.AreEqual(product2.Name, actualProduct2.Name);
        Assert.AreEqual(product2.Price, actualProduct2.Price);
        Assert.AreEqual(targetCategory.Name, actualProduct2.ProductCategory.Name);
        Assert.AreEqual(120, actualProduct2.ProductStock.Quantity);
    }

    [TestMethod(DisplayName = "指定カテゴリに商品が存在しない場合は空のリストが返る")]
    public async Task SelectByCategoryAsync_WhenCategoryHasNoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        var category = await InsertCategoryAsync("商品なしカテゴリ");

        // Act
        var products = await _productRepository.SelectByCategoryAsync(
            category.CategoryUuid.ToString());

        // Assert
        Assert.IsNotNull(products);
        Assert.IsEmpty(products);
    }

    [TestMethod(DisplayName = "カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task SelectByCategoryAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.SelectByCategoryAsync("abc"));

        // Assert
        Assert.AreEqual(
            "カテゴリUUIDの形式が不正です。",
            exception.Message);
    }

    [TestMethod(DisplayName = "商品名が存在するとtrueが返る")]
    public async Task ExistsByNameAsync_WhenNameExists_ShouldReturnTrue()
    {
        // Arrange
        var category = await InsertCategoryAsync("存在確認カテゴリ");
        var product = await InsertProductAsync(
            category,
            CreateUniqueProductName("存在商品"),
            1000);
        await InsertStockAsync(product, 10);

        // Act
        var result = await _productRepository.ExistsByNameAsync(product.Name);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "商品名が存在しないとfalseが返る")]
    public async Task ExistsByNameAsync_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingName = CreateUniqueProductName("不存在商品");

        // Act
        var result = await _productRepository.ExistsByNameAsync(
            nonExistingName);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "削除済みの商品名の場合はfalseが返る")]
    public async Task ExistsByNameAsync_WhenProductIsDeleted_ShouldReturnFalse()
    {
        // Arrange
        var category = await InsertCategoryAsync("削除商品カテゴリ");
        var product = await InsertProductAsync(
            category,
            CreateUniqueProductName("旧型商品"),
            1000,
            deleteFlag: 1);
        await InsertStockAsync(product, 10);

        // Act
        var result = await _productRepository.ExistsByNameAsync(product.Name);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "商品と商品在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistProductAndStock()
    {
        // Arrange
        var categoryEntity = await InsertCategoryAsync("文具");

        var category = new ProductCategory(
            categoryEntity.CategoryUuid.ToString(),
            categoryEntity.Name);

        var stock = ProductStock.Create(10);
        var product = Product.Create(
            CreateUniqueProductName("鉛筆削り"),
            1000,
            "https://example.com/test.png",
            category,
            stock);

        _productUuids.Add(Guid.Parse(product.ProductUuid));
        _stockUuids.Add(Guid.Parse(product.ProductStock.StockUuid));

        // Act
        await _productRepository.CreateAsync(product);

        var result = await _productRepository.SelectByUuidAsync(
            product.ProductUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);
        Assert.AreEqual(product.ImageUrl, result.ImageUrl);
        Assert.AreEqual(product.DeleteFlg, result.DeleteFlg);
        Assert.AreEqual(
            product.ProductStock.StockUuid,
            result.ProductStock.StockUuid);
        Assert.AreEqual(
            product.ProductStock.Quantity,
            result.ProductStock.Quantity);
        Assert.AreEqual(
            product.ProductCategory.CategoryUuid,
            result.ProductCategory.CategoryUuid);
        Assert.AreEqual(
            product.ProductCategory.Name,
            result.ProductCategory.Name);
    }

    [TestMethod(DisplayName = "商品カテゴリが存在しない場合はInternalExceptionをスローする")]
    public async Task CreateAsync_WhenCategoryDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid().ToString(),
            "存在しないカテゴリ");

        var stock = ProductStock.Create(10);
        var product = Product.Create(
            CreateUniqueProductName("リップ"),
            1000,
            "https://example.com/test.png",
            category,
            stock);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.CreateAsync(product));

        // Assert
        Assert.AreEqual(
            "指定された商品カテゴリが存在しません。",
            exception.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task CreateAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory("abc", "カテゴリ");
        var stock = ProductStock.Create(10);
        var product = Product.Create(
            CreateUniqueProductName("テスト商品"),
            1000,
            "https://example.com/test.png",
            category,
            stock);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.CreateAsync(product));

        // Assert
        Assert.AreEqual(
            "商品カテゴリUUIDの形式が不正です。",
            exception.Message);
    }

    [TestMethod(DisplayName = "存在する商品を更新するとtrueが返り、商品情報と在庫が更新される")]
    public async Task UpdateAsync_WhenProductExists_ShouldUpdateProductAndStock()
    {
        // Arrange
        var oldCategory = await InsertCategoryAsync("更新前カテゴリ");
        var existingProduct = await InsertProductAsync(
            oldCategory,
            CreateUniqueProductName("更新前商品"),
            1000);
        await InsertStockAsync(existingProduct, 10);

        var newCategoryEntity = await InsertCategoryAsync("更新後カテゴリ");
        var newCategory = new ProductCategory(
            newCategoryEntity.CategoryUuid.ToString(),
            newCategoryEntity.Name);

        var newStock = ProductStock.Create(50);
        var updateProduct = Product.Restore(
            existingProduct.ProductUuid.ToString(),
            CreateUniqueProductName("万年筆"),
            5000,
            "https://example.com/updated-product.png",
            newCategory,
            0,
            newStock);

        // Act
        var result = await _productRepository.UpdateAsync(updateProduct);

        // Assert
        Assert.IsTrue(result);

        var updatedResult = await _productRepository.SelectByUuidAsync(
            existingProduct.ProductUuid.ToString());

        Assert.IsNotNull(updatedResult);
        Assert.AreEqual(updateProduct.ProductUuid, updatedResult.ProductUuid);
        Assert.AreEqual(updateProduct.Name, updatedResult.Name);
        Assert.AreEqual(updateProduct.Price, updatedResult.Price);
        Assert.AreEqual(updateProduct.ImageUrl, updatedResult.ImageUrl);
        Assert.AreEqual(
            newCategory.CategoryUuid,
            updatedResult.ProductCategory.CategoryUuid);
        Assert.AreEqual(
            newCategory.Name,
            updatedResult.ProductCategory.Name);
        Assert.AreEqual(50, updatedResult.ProductStock.Quantity);
    }

    [TestMethod(DisplayName = "更新対象の商品が存在しない場合はfalseを返す")]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var categoryEntity = await InsertCategoryAsync("既存カテゴリ");
        var existingCategory = new ProductCategory(
            categoryEntity.CategoryUuid.ToString(),
            categoryEntity.Name);

        var stock = ProductStock.Create(10);
        var product = Product.Restore(
            Guid.NewGuid().ToString(),
            CreateUniqueProductName("不存在商品"),
            1000,
            "https://example.com/not-exist.png",
            existingCategory,
            0,
            stock);

        // Act
        var result = await _productRepository.UpdateAsync(product);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "更新先の商品カテゴリが存在しない場合はInternalExceptionをスローする")]
    public async Task UpdateAsync_WhenCategoryDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var currentCategory = await InsertCategoryAsync("現在カテゴリ");
        var existingProduct = await InsertProductAsync(
            currentCategory,
            CreateUniqueProductName("更新対象商品"),
            1000);
        await InsertStockAsync(existingProduct, 10);

        var nonExistingCategory = new ProductCategory(
            Guid.NewGuid().ToString(),
            "存在しないカテゴリ");

        var stock = ProductStock.Create(20);
        var product = Product.Restore(
            existingProduct.ProductUuid.ToString(),
            CreateUniqueProductName("更新商品"),
            2000,
            "https://example.com/category-not-found.png",
            nonExistingCategory,
            0,
            stock);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.UpdateAsync(product));

        // Assert
        Assert.AreEqual(
            "指定された商品カテゴリが存在しません。",
            exception.Message);
    }

    [TestMethod(DisplayName = "商品UUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task UpdateAsync_WhenProductUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid().ToString(),
            "カテゴリ");
        var stock = ProductStock.Create(10);
        var product = Product.Restore(
            "abc",
            CreateUniqueProductName("テスト商品"),
            1000,
            "https://example.com/test.png",
            category,
            0,
            stock);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.UpdateAsync(product));

        // Assert
        Assert.AreEqual(
            "商品UUIDの形式が不正です。",
            exception.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task UpdateAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        var currentCategory = await InsertCategoryAsync("現在カテゴリ");
        var existingProduct = await InsertProductAsync(
            currentCategory,
            CreateUniqueProductName("対象商品"),
            1000);
        await InsertStockAsync(existingProduct, 10);

        var invalidCategory = new ProductCategory("abc", "カテゴリ");
        var stock = ProductStock.Create(10);
        var product = Product.Restore(
            existingProduct.ProductUuid.ToString(),
            CreateUniqueProductName("テスト商品"),
            1000,
            "https://example.com/test.png",
            invalidCategory,
            0,
            stock);

        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.UpdateAsync(product));

        // Assert
        Assert.AreEqual(
            "商品カテゴリUUIDの形式が不正です。",
            exception.Message);
    }

    [TestMethod(DisplayName = "存在する商品を削除するとtrueが返り、取得できなくなる")]
    public async Task DeleteAsync_WhenProductExists_ShouldReturnTrueAndNotBeSelectable()
    {
        // Arrange
        var category = await InsertCategoryAsync("削除テストカテゴリ");
        var product = await InsertProductAsync(
            category,
            CreateUniqueProductName("削除テスト商品"),
            1000);
        await InsertStockAsync(product, 10);

        // Act
        var result = await _productRepository.DeleteAsync(
            product.ProductUuid.ToString());

        // Assert
        Assert.IsTrue(result);

        var deletedProduct = await _productRepository.SelectByUuidAsync(
            product.ProductUuid.ToString());

        Assert.IsNull(deletedProduct);

        var deletedEntity = await _dbContext.Products
            .AsNoTracking()
            .SingleAsync(entity =>
                entity.ProductUuid == product.ProductUuid);

        Assert.AreEqual(1, deletedEntity.DeleteFlag);
    }

    [TestMethod(DisplayName = "商品が存在しない場合はfalseを返す")]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        // Act
        var result = await _productRepository.DeleteAsync(productUuid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "商品UUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task DeleteAsync_WhenProductUuidIsInvalid_ShouldThrowInternalException()
    {
        // Act
        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => _productRepository.DeleteAsync("abc"));

        // Assert
        Assert.AreEqual(
            "商品UUIDの形式が不正です。",
            exception.Message);
    }

    private async Task<ProductCategoryEntity> InsertCategoryAsync(
        string name)
    {
        var category = new ProductCategoryEntity
        {
            CategoryUuid = Guid.NewGuid(),
            Name = CreateUniqueCategoryName(name)
        };

        _categoryUuids.Add(category.CategoryUuid);

        await _dbContext.ProductCategories.AddAsync(category);
        await _dbContext.SaveChangesAsync();

        return category;
    }

    private async Task<ProductEntity> InsertProductAsync(
        ProductCategoryEntity category,
        string name,
        int price,
        int deleteFlag = 0)
    {
        var product = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            ProductCategoryId = category.Id,
            ProductCategory = category,
            Name = name,
            Price = price,
            ImageUrl = "https://example.com/test.png",
            DeleteFlag = deleteFlag
        };

        _productUuids.Add(product.ProductUuid);

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        return product;
    }

    private async Task<ProductStockEntity> InsertStockAsync(
        ProductEntity product,
        int quantity)
    {
        var stock = new ProductStockEntity
        {
            StockUuid = Guid.NewGuid(),
            ProductId = product.Id,
            Product = product,
            Quantity = quantity
        };

        _stockUuids.Add(stock.StockUuid);

        await _dbContext.ProductStocks.AddAsync(stock);
        await _dbContext.SaveChangesAsync();

        return stock;
    }

    private static string CreateUniqueCategoryName(string prefix)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var maxPrefixLength = 30 - suffix.Length - 1;
        var safePrefix = prefix[..Math.Min(prefix.Length, maxPrefixLength)];
        return $"{safePrefix}_{suffix}";
    }

    private static string CreateUniqueProductName(string prefix)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var maxPrefixLength = 20 - suffix.Length - 1;
        var safePrefix = prefix[..Math.Min(prefix.Length, maxPrefixLength)];
        return $"{safePrefix}_{suffix}";
    }
}