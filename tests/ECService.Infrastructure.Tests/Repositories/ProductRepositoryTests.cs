using ECService.Domain.Repositories;
using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Repositories;
using ECService.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using ECService.Infrastructure.Exceptions;
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
        const string connectionString =
            "Host=localhost;" +
            "Port=5432;" +
            "Database=ec_service_manage_db_test;" +
            "Username=postgres;" +
            "Password=training";

        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

        _dbContext = new AppDbContext(options);

        if (!await _dbContext.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用データベースに接続できません。");
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
        Assert.AreEqual("8edb1714-54ad-4349-a64c-0341466ac94a", product4.Name);
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
    [TestMethod(DisplayName = "書籍と書籍在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistBoth()
    {
        // 登録データを用意する
        var productCategory = new ProductCategory("e269c98c-61b7-4ca7-9fae-ecd74234989e", "児童書");
        var productStock = new ProductStock(Guid.NewGuid().ToString(), 20);
        var product = new Product(Guid.NewGuid().ToString(), "書籍-A", "草間彌生");
        product.ChangeStock(productStock);
        product.ChangeCategory(productCategory);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 書籍と書籍在庫を永続化する
                await _productRepository.CreateAsync(product);
                // 登録された書籍と書籍在庫を取得して値を検証する
                var result = await _productRepository
                     .SelectByIdWithProductStockAndCategoryAsync(product.ProductUuid);
                // nullでないことを検証する
                Assert.IsNotNull(result);
                // 書籍Idを検証する
                Assert.AreEqual(result.ProductUuid, product.ProductUuid);
                // 図書名を検証する
                Assert.AreEqual(result.Title, product.Title);
                // 単価を検証する
                Assert.AreEqual(result.Author, product.Author);
                // 書籍在庫がnullでないことを検証する
                Assert.IsNotNull(result.ProductStock);
                // 書籍在庫Idを検証する
                Assert.AreEqual(result.ProductStock.StockUuid, product.ProductStock!.StockUuid);
                // 在庫数を検証する
                Assert.AreEqual(result.ProductStock.Stock, product.ProductStock.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }
    [TestMethod(DisplayName = "図書名が存在するとtrueが返される")]
    public async Task ExistsByTitle_WhenTitleExists_ShouldReturnTrue()
    {
        var result = await _productRepository.ExistsByTitleAsync("はらぺこあおむし");
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "図書名が存在しないとfalseが返される")]
    public async Task ExistsByTitle_WhenTitleDoesNotExist_ShouldReturnFalse()
    {
        var result = await _productRepository.ExistsByTitleAsync("かいけつゾロリ");
        Assert.IsFalse(result);
    }
    [TestMethod(DisplayName = "存在する書籍のキーワードを指定すると、該当する書籍のリストが返される")]
    public async Task SelectByTitleLikeWithProductStockAndCategoryAsync_WithExistingKeyword_ShouldReturnMatchingProducts()
    {
        var products = await _productRepository
        .SelectByTitleLikeWithProductStockAndCategoryAsync("はらぺこ");
        // nullでないことを検証する
        Assert.IsNotNull(products);
        // 件数が4件であることを検証する
        Assert.AreEqual(1, products.Count);
    }
    [TestMethod(DisplayName = "存在しない書籍のキーワードを指定すると、空の書籍のリストが返される")]
    public async Task SelectByTitleLikeWithProductStockAndCategoryAsync_WithNonExistingKeyword_ShouldReturnEmptyList()
    {
        var products = await _productRepository
            .SelectByTitleLikeWithProductStockAndCategoryAsync("書籍-X");
        // nullでないことを検証する
        Assert.IsNotNull(products);
        // 件数が0であることを検証する
        Assert.AreEqual(0, products.Count);
    }
    [TestMethod(DisplayName = "存在する書籍を変更するとtrueが返される")]
    public async Task UpdateProduct_WhenProductExists_ShouldReturnTrue()
    {
        // 変更データを準備する
        var productStock = new ProductStock("8311a860-c63f-45d5-9b42-3bfd6ef886f3", 10);
        var product = new Product("64b25512-6dfc-4034-9372-9030f118bdb9", "はらぺこあおむし", "エリック・カール");
        product.ChangeStock(productStock);

        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 書籍を変更する
                var result = await _productRepository.UpdateByIdAsync(product);
                // trueであることを検証する
                Assert.IsTrue(result);
                // 変更された書籍を取得する
                var updateResult = await _productRepository
                    .SelectByIdWithProductStockAndCategoryAsync(product.ProductUuid);
                // 図書名を検証する
                Assert.AreEqual(product.Title, updateResult!.Title);
                // 単価を検証する
                Assert.AreEqual(product.Author, updateResult!.Author);
                // 書籍在庫数を検証する
                Assert.AreEqual(product.ProductStock!.Stock, updateResult.ProductStock!.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }

    [TestMethod(DisplayName = "存在しない書籍を変更するとfalseが返される")]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // 変更データを準備する
        var productStock = new ProductStock("828fb567-6f6b-11f0-954a-00155d1bd30a", 50);
        var product = new Product("ac413f22-0cf1-490a-9635-7e9ca810e555", "かいけつゾロリ", "草間彌生");
        product.ChangeStock(productStock);
        // 書籍を変更する
        var result = await _productRepository.UpdateByIdAsync(product);
        // falseが返されることを検証する
        Assert.IsFalse(result);
    }

}