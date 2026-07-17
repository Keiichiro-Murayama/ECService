using ECService.Domain.Repositories;
using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Repositories;
using ECService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using ECService.Infrastructure.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

[TestClass]
[TestCategory("Repositories")]
[DoNotParallelize]
public class ProductRepositoryTests
{
    private AppDbContext _dbContext = null!;
    private IProductRepository _productRepository = null!;

    [TestInitialize]
    public async Task TestInit()
    {
        var config = new ConfigurationBuilder()
     .SetBasePath(AppContext.BaseDirectory)
     .AddJsonFile("appsettingsTests.json", optional: false)
     .Build();

        var connectionString =
            config.GetConnectionString("ECServiceDB")
            ?? throw new InvalidOperationException(
                "接続文字列 ECServiceDB が見つかりません。");

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _dbContext = new AppDbContext(options);

        if (!await _dbContext.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用PostgreSQLへ接続できません。");
        }
        var categoryAdapter =
            new ProductCategoryEntityAdapter();

        var stockAdapter =
            new ProductStockEntityAdapter();

        var factory =
            new ProductFactory(
                categoryAdapter,
                stockAdapter);

        var productEntityAdapter =
            new ProductEntityAdapter();

        _productRepository =
            new ProductRepository(
                _dbContext,
                factory,
                productEntityAdapter,
                stockAdapter);
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (_dbContext is not null)
        {
            await _dbContext.DisposeAsync();
        }
    }

    [TestMethod(DisplayName = "削除されていない商品一覧を取得できる")]
    public async Task SelectAllAsync_ShouldReturnAllActiveProducts()
    {
        // 商品一覧を取得する
        var products = await _productRepository.SelectAllAsync();

        // nullでないことを確認
        Assert.IsNotNull(products);

        // 未削除の商品件数を確認
        Assert.HasCount(6, products);

        // 商品1が含まれることを確認
        var product1 = products.Single(
            product => product.ProductUuid ==
                "11e6d2e9-e24d-4078-9552-3c5efaf62fdf");

        Assert.AreEqual("高級ボールペン", product1.Name);
        Assert.AreEqual(1200, product1.Price);
        Assert.AreEqual(0, product1.DeleteFlg);

        // 商品2が含まれることを確認
        var product2 = products.Single(
            product => product.ProductUuid ==
                "e9ac3ed7-f21e-4bde-afc1-de9add2bdd41");

        Assert.AreEqual("エコバッグ", product2.Name);
        Assert.AreEqual(880, product2.Price);
        Assert.AreEqual(0, product2.DeleteFlg);

        // 商品3が含まれることを確認
        var product3 = products.Single(
            product => product.ProductUuid ==
                "974f8010-9ad1-4cce-b1c3-72f2d8e45e43");
        Assert.AreEqual("アロマキャンドル", product3.Name);
        Assert.AreEqual(1500, product3.Price);
        Assert.AreEqual(0, product3.DeleteFlg);

        // 商品4が含まれることを確認
        var product4 = products.Single(
            product => product.ProductUuid ==
                "8edb1714-54ad-4349-a64c-0341466ac94a");
        Assert.AreEqual("Type-C ハブ 6in1", product4.Name);
        Assert.AreEqual(3980, product4.Price);
        Assert.AreEqual(0, product4.DeleteFlg);

        // 商品5が含まれることを確認
        var product5 = products.Single(
            product => product.ProductUuid ==
                "cf037f04-4992-42ef-8bbb-98673144f8b1");
        Assert.AreEqual("充電式ワイヤレスマウス", product5.Name);
        Assert.AreEqual(2480, product5.Price);
        Assert.AreEqual(0, product5.DeleteFlg);

        // 商品6が含まれることを確認
        var product6 = products.Single(
            product => product.ProductUuid ==
                "eaec2628-9aea-42e3-9102-8cfaa8bb9484");
        Assert.AreEqual("耐水ノート(A5)", product6.Name);
        Assert.AreEqual(450, product6.Price);
        Assert.AreEqual(0, product6.DeleteFlg);

    }

    [TestMethod(DisplayName = "削除済み商品は商品一覧に含まれない")]
    public async Task SelectAllAsync_WhenDeleteFlagIsOne_ShouldNotReturnProduct()
    {
        // Arrange
        const string deletedProductUuid =
            "c87a242f-aa62-48f2-ab04-e73c41447916";

        // Act
        var products = await _productRepository.SelectAllAsync();

        // Assert
        Assert.IsNotNull(products);

        Assert.IsFalse(
            products.Any(product =>
                product.ProductUuid == deletedProductUuid));
    }
    [TestMethod(DisplayName = "指定カテゴリの商品一覧を取得できる")]
    public async Task SelectByCategoryAsync_WhenCategoryExists_ShouldReturnProducts()
    {
        // Arrange
        const string categoryUuid =
            "4cfdd7d3-d002-40de-b4a9-ecffd3869924";

        // Act
        var products =
            await _productRepository.SelectByCategoryAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(products);

        // 対象カテゴリの商品件数に合わせて変更する
        Assert.HasCount(2, products);

        // 取得された商品がすべて指定カテゴリであることを確認
        Assert.IsTrue(
            products.All(product =>
                product.ProductCategory.CategoryUuid == categoryUuid));

        // 1件目の商品をUUIDで取得
        var product1 = products.Single(
            product =>
                product.ProductUuid ==
                "11e6d2e9-e24d-4078-9552-3c5efaf62fdf");

        Assert.AreEqual("高級ボールペン", product1.Name);
        Assert.AreEqual(1200, product1.Price);
        Assert.AreEqual(categoryUuid, product1.ProductCategory.CategoryUuid);
        Assert.AreEqual("文具", product1.ProductCategory.Name);
        Assert.IsNotNull(product1.ProductStock);
        Assert.AreEqual(50, product1.ProductStock.Quantity);

        // 2件目の商品をUUIDで取得
        var product2 = products.Single(
            product =>
                product.ProductUuid ==
                "eaec2628-9aea-42e3-9102-8cfaa8bb9484");

        Assert.AreEqual("耐水ノート(A5)", product2.Name);
        Assert.AreEqual(450, product2.Price);
        Assert.AreEqual(categoryUuid, product2.ProductCategory.CategoryUuid);
        Assert.AreEqual("文具", product2.ProductCategory.Name);
        Assert.IsNotNull(product2.ProductStock);
        Assert.AreEqual(120, product2.ProductStock.Quantity);
    }
    [TestMethod(DisplayName = "指定カテゴリに商品が存在しない場合は空のリストが返る")]
    public async Task SelectByCategoryAsync_WhenCategoryHasNoProducts_ShouldReturnEmptyList()
    {
        var strategy = _dbContext!.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Arrange
                // 商品を紐付けないカテゴリを作成する
                var category = new ProductCategoryEntity
                {
                    CategoryUuid = Guid.NewGuid(),
                    Name = "化粧品"
                };

                await _dbContext.ProductCategories.AddAsync(category);
                await _dbContext.SaveChangesAsync();

                // Act
                var products =
                    await _productRepository.SelectByCategoryAsync(
                        category.CategoryUuid.ToString());

                // Assert
                Assert.IsNotNull(products);
                Assert.IsEmpty(products);
            }
            finally
            {
                // 作成したカテゴリをDBに残さない
                await tx.RollbackAsync();
            }
        });
    }
    [TestMethod(DisplayName = "カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task SelectByCategoryAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        try
        {
            // Act
            await _productRepository.SelectByCategoryAsync("abc");

            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "カテゴリUUIDの形式が不正です。",
                ex.Message);
        }
    }
    [TestMethod(DisplayName = "商品名が存在するとtrueが返る")]
    public async Task ExistsByNameAsync_WhenNameExists_ShouldReturnTrue()
    {
        // Act
        var result = await _productRepository.ExistsByNameAsync("高級ボールペン");

        // Assert
        Assert.IsTrue(result);
    }
    [TestMethod(DisplayName = "商品名が存在しないとfalseが返る")]
    public async Task ExistsByNameAsync_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        // Act
        var result = await _productRepository.ExistsByNameAsync("弁当箱");

        // Assert
        Assert.IsFalse(result);
    }
    [TestMethod(DisplayName = "削除済みの商品名の場合はfalseが返る")]
    public async Task ExistsByNameAsync_WhenProductIsDeleted_ShouldReturnFalse()
    {
        var result = await _productRepository.ExistsByNameAsync("旧型USBメモリ 8GB");

        Assert.IsFalse(result);
    }
    [TestMethod(DisplayName = "商品と商品在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistProductAndStock()
    {
        // 登録データを準備する
        var category = new ProductCategory(
            "4cfdd7d3-d002-40de-b4a9-ecffd3869924",
            "文具");

        var stock = ProductStock.Create(10);

        var product = Product.Create(
            "えんぴつ削り",
            1000,
            "https://example.com/test.png",
            category,
            stock);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Act
                await _productRepository.CreateAsync(product);

                // 登録した商品を取得する
                var result = await _productRepository
                    .SelectByUuidAsync(product.ProductUuid);

                // Assert
                Assert.IsNotNull(result);
                // 商品情報
                Assert.AreEqual(product.ProductUuid, result.ProductUuid);
                Assert.AreEqual(product.Name, result.Name);
                Assert.AreEqual(product.Price, result.Price);
                Assert.AreEqual(product.ImageUrl, result.ImageUrl);
                Assert.AreEqual(product.DeleteFlg, result.DeleteFlg);
                // 在庫情報
                Assert.IsNotNull(result.ProductStock);
                Assert.AreEqual(product.ProductStock.StockUuid, result.ProductStock.StockUuid);
                Assert.AreEqual(product.ProductStock.Quantity, result.ProductStock.Quantity);
                // カテゴリ情報
                Assert.IsNotNull(result.ProductCategory);
                Assert.AreEqual(product.ProductCategory.CategoryUuid, result.ProductCategory.CategoryUuid);

                Assert.AreEqual(product.ProductCategory.Name, result.ProductCategory.Name);
            }
            finally
            {
                await tx.RollbackAsync();
            }
        });
    }
    [TestMethod(DisplayName = "商品カテゴリが存在しない場合はInternalExceptionをスローする")]
    public async Task CreateAsync_WhenCategoryDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory(
            "11111111-1111-1111-1111-111111111111",
            "化粧品");

        var stock = ProductStock.Create(10);

        var product = Product.Create(
            "リップスティック",
            1000,
            "https://example.com/test.png",
            category,
            stock);

        // Act
        try
        {
            await _productRepository.CreateAsync(product);
            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "指定された商品カテゴリが存在しません。",
                ex.Message);
        }
    }
    [TestMethod(DisplayName = "商品カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task CreateAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory(
            "abc",
            "カテゴリ");

        var stock = ProductStock.Create(10);

        var product = Product.Create(
            "テスト商品",
            1000,
            "https://example.com/test.png",
            category,
            stock);

        // Act
        try
        {
            await _productRepository.CreateAsync(product);
            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "商品カテゴリUUIDの形式が不正です。",
                ex.Message);
        }
    }
    [TestMethod(DisplayName = "存在する商品を更新するとtrueが返り、商品情報と在庫が更新される")]
    public async Task UpdateAsync_WhenProductExists_ShouldUpdateProductAndStock()
    {
        // Arrange
        // DBに実在する更新対象の商品UUID
        const string productUuid =
            "e9ac3ed7-f21e-4bde-afc1-de9add2bdd41";

        // DBに実在する変更後カテゴリ
        var newCategory = new ProductCategory(
            "31c799e5-7538-4205-b95d-818946316365",
            "雑貨");

        // 変更後の在庫
        var newStock = ProductStock.Create(50);

        // 更新後の商品ドメインを作成
        var updateProduct = Product.Restore(
            productUuid,
            "万年筆",
            5000,
            "https://example.com/updated-product.png",
            newCategory,
            0,
            newStock);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Act
                var result =
                    await _productRepository.UpdateAsync(updateProduct);

                // Assert
                // 更新成功の場合はtrue
                Assert.IsTrue(result);

                // 更新後の商品を取得
                var updatedResult =
                    await _productRepository.SelectByUuidAsync(productUuid);

                Assert.IsNotNull(updatedResult);

                // 商品UUID
                Assert.AreEqual(productUuid, updatedResult.ProductUuid);
                // 商品名
                Assert.AreEqual("万年筆", updatedResult.Name);
                // 価格
                Assert.AreEqual(5000, updatedResult.Price);
                // 画像URL
                Assert.AreEqual("https://example.com/updated-product.png", updatedResult.ImageUrl);
                // カテゴリ
                Assert.IsNotNull(updatedResult.ProductCategory);
                Assert.AreEqual(newCategory.CategoryUuid, updatedResult.ProductCategory.CategoryUuid);
                Assert.AreEqual(newCategory.Name, updatedResult.ProductCategory.Name);
                // 在庫
                Assert.IsNotNull(updatedResult.ProductStock);
                Assert.AreEqual(50, updatedResult.ProductStock.Quantity);
            }
            finally
            {
                // DBの変更を元に戻す
                await tx.RollbackAsync();
            }
        });
    }
    [TestMethod(DisplayName = "更新対象の商品が存在しない場合はfalseを返す")]
    public async Task UpdateAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var existingCategory = new ProductCategory(
            "4cfdd7d3-d002-40de-b4a9-ecffd3869924",
            "文具");

        var stock = ProductStock.Create(10);

        var product = Product.Restore(
            Guid.NewGuid().ToString(), // DBに存在しない商品UUID
            "万年筆",
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
        const string existingProductUuid =
            "11e6d2e9-e24d-4078-9552-3c5efaf62fdf";

        var nonExistingCategory = new ProductCategory(
            Guid.NewGuid().ToString(), // DBに存在しないカテゴリUUID
            "化粧品");

        var stock = ProductStock.Create(20);

        var product = Product.Restore(
            existingProductUuid,
            "万年筆",
            2000,
            "https://example.com/category-not-found.png",
            nonExistingCategory,
            0,
            stock);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
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
            "4cfdd7d3-d002-40de-b4a9-ecffd3869924",
            "文具");

        var stock = ProductStock.Create(10);

        var product = Product.Restore(
            "abc",   // UUID形式が不正
            "テスト商品",
            1000,
            "https://example.com/test.png",
            category,
            0,
            stock);

        // Act
        try
        {
            await _productRepository.UpdateAsync(product);

            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "商品UUIDの形式が不正です。",
                ex.Message);
        }
    }
    [TestMethod(DisplayName = "商品カテゴリUUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task UpdateAsync_WhenCategoryUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        var category = new ProductCategory(
            "abc",   // UUID形式が不正
            "カテゴリ");

        var stock = ProductStock.Create(10);

        var product = Product.Restore(
            "11e6d2e9-e24d-4078-9552-3c5efaf62fdf",
            "テスト商品",
            1000,
            "https://example.com/test.png",
            category,
            0,
            stock);

        // Act
        try
        {
            await _productRepository.UpdateAsync(product);

            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "商品カテゴリUUIDの形式が不正です。",
                ex.Message);
        }
    }
    [TestMethod(DisplayName = "存在する商品を削除するとtrueが返り、取得できなくなる")]
    public async Task DeleteAsync_WhenProductExists_ShouldReturnTrueAndNotBeSelectable()
    {
        // Arrange
        const string productUuid =
            "11e6d2e9-e24d-4078-9552-3c5efaf62fdf";

        var strategy = _dbContext!.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var tx =
                await _dbContext.Database.BeginTransactionAsync();

            try
            {
                // Act
                var result =
                    await _productRepository.DeleteAsync(productUuid);

                // Assert
                Assert.IsTrue(result);

                // 論理削除後はSelectByUuidAsyncで取得できない
                var deletedProduct =
                    await _productRepository.SelectByUuidAsync(productUuid);

                Assert.IsNull(deletedProduct);

                // DB上ではDeleteFlagが1になっていることを確認
                var productEntity = await _dbContext!.Products
                    .AsNoTracking()
                    .SingleAsync(product =>
                        product.ProductUuid == Guid.Parse(productUuid));

                Assert.AreEqual(1, productEntity.DeleteFlag);
            }
            finally
            {
                await tx.RollbackAsync();
            }
        });
    }
    [TestMethod(DisplayName = "商品が存在しない場合はfalseを返す")]
    public async Task DeleteAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        const string productUuid =
            "11111111-1111-1111-1111-111111111111";

        // Act
        var result = await _productRepository.DeleteAsync(productUuid);

        // Assert
        Assert.IsFalse(result);
    }
    [TestMethod(DisplayName = "商品UUIDの形式が不正な場合はInternalExceptionをスローする")]
    public async Task DeleteAsync_WhenProductUuidIsInvalid_ShouldThrowInternalException()
    {
        // Arrange
        const string productUuid = "abc";

        try
        {
            // Act
            await _productRepository.DeleteAsync(productUuid);

            Assert.Fail("InternalExceptionが発生しませんでした。");
        }
        catch (InternalException ex)
        {
            // Assert
            Assert.AreEqual(
                "商品UUIDの形式が不正です。",
                ex.Message);
        }
    }
}