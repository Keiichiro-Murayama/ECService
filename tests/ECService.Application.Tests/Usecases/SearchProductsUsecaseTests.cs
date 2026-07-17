using System.Reflection;
using System.Runtime.CompilerServices;
using ECService.Application.Usecases.Imps;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// SearchProductsUsecase の単体テスト
///
/// 商品検索機能において、カテゴリUUIDの有無による検索処理の分岐、
/// カテゴリUUIDの形式チェック、カテゴリ存在チェックを検証する。
/// </summary>
[TestClass]
public class SearchProductsUsecaseTests
{
    /// <summary>
    /// テスト出力用
    /// </summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// ターミナルとテスト結果にログを出力する
    /// </summary>
    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    /// <summary>
    /// categoryUuid が null の場合、全商品検索が呼ばれること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsNull_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsNull_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
                name: "エコバッグ",
                price: 880,
                imageUrl: "https://example.com/images/bag.jpg")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("エコバッグ", result[0].Name);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("categoryUuid が null の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    /// <summary>
    /// categoryUuid が空文字の場合、全商品検索が呼ばれること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsEmpty_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsEmpty_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                name: "耐水ノート",
                price: 450,
                imageUrl: "https://example.com/images/note.jpg")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync("");

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("耐水ノート", result[0].Name);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("categoryUuid が空文字の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    /// <summary>
    /// categoryUuid が空白の場合、全商品検索が呼ばれること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsWhiteSpace_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsWhiteSpace_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "45c24c9f-a494-4e75-afb8-794c5c66135f",
                name: "充電式マウス",
                price: 2480,
                imageUrl: "https://example.com/images/mouse.jpg")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync("   ");

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("充電式マウス", result[0].Name);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("categoryUuid が空白の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    /// <summary>
    /// categoryUuid が指定され、カテゴリが存在する場合、カテゴリ検索が呼ばれること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidExists_CallsSelectByCategoryAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidExists_CallsSelectByCategoryAsync：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var categories = new List<ProductCategory>
        {
            CreateProductCategory(
                categoryUuid: categoryUuid,
                name: "雑貨")
        };

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "eb07baff-7f28-4356-abfb-020c31e04dc7",
                name: "Type-Cハブ",
                price: 3980,
                imageUrl: "https://example.com/images/hub.jpg")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(categories);

        productRepositoryMock
            .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(1, result);
        Assert.AreEqual("Type-Cハブ", result[0].Name);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(categoryUuid),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("categoryUuid が指定され、カテゴリが存在する場合、SelectByCategoryAsync が1回呼ばれることを確認しました。");
    }

    /// <summary>
    /// 全商品検索結果が0件の場合、空リストが返ること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_SelectAllReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        Log("ExecuteAsync_SelectAllReturnsEmptyList_ReturnsEmptyList：テスト開始");

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<Product>());

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("全商品検索結果が0件の場合、空のリストが返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリ検索結果が0件の場合、空リストが返ること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_SelectByCategoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        Log("ExecuteAsync_SelectByCategoryReturnsEmptyList_ReturnsEmptyList：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var categories = new List<ProductCategory>
        {
            CreateProductCategory(
                categoryUuid: categoryUuid,
                name: "雑貨")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(categories);

        productRepositoryMock
            .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(categoryUuid),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        Log("カテゴリが存在し、検索結果が0件の場合、空のリストが返ることを確認しました。");
    }

    /// <summary>
    /// categoryUuid がUUID形式ではない場合、DomainExceptionが発生すること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsInvalidFormat_ThrowsDomainException()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsInvalidFormat_ThrowsDomainException：テスト開始");

        var invalidCategoryUuid = "invalid-category-uuid";

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsExactlyAsync<DomainException>(async () =>
            await usecase.ExecuteAsync(invalidCategoryUuid));

        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            exception.Message);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        Log("categoryUuid がUUID形式ではない場合、DomainException が発生することを確認しました。");
    }

    /// <summary>
    /// categoryUuid がカテゴリマスタに存在しない場合、DomainExceptionが発生すること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidDoesNotExist_ThrowsDomainException()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidDoesNotExist_ThrowsDomainException：テスト開始");

        var notExistsCategoryUuid = "99999999-9999-9999-9999-999999999999";

        var categories = new List<ProductCategory>
        {
            CreateProductCategory(
                categoryUuid: "11111111-1111-1111-1111-111111111111",
                name: "雑貨")
        };

        var productRepositoryMock = new Mock<IProductRepository>();
        var productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(categories);

        var usecase = new SearchProductsUsecase(
            productRepositoryMock.Object,
            productCategoryRepositoryMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsExactlyAsync<DomainException>(async () =>
            await usecase.ExecuteAsync(notExistsCategoryUuid));

        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            exception.Message);

        productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        productRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        productRepositoryMock.Verify(
            repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
            Times.Never);

        Log("categoryUuid がカテゴリマスタに存在しない場合、DomainException が発生することを確認しました。");
    }

    /// <summary>
    /// テスト用のProductを作成する
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product =
            (Product)RuntimeHelpers.GetUninitializedObject(typeof(Product));

        SetProperty(product, nameof(Product.ProductUuid), productUuid);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.ImageUrl), imageUrl);

        return product;
    }

    /// <summary>
    /// テスト用のProductCategoryを作成する
    /// </summary>
    private static ProductCategory CreateProductCategory(
        string categoryUuid,
        string name)
    {
        var category =
            (ProductCategory)RuntimeHelpers.GetUninitializedObject(typeof(ProductCategory));

        SetProperty(category, nameof(ProductCategory.CategoryUuid), categoryUuid);
        SetProperty(category, nameof(ProductCategory.Name), name);

        return category;
    }

    /// <summary>
    /// private set のプロパティにテスト用の値を設定する
    /// </summary>
    private static void SetProperty<T>(
        T target,
        string propertyName,
        object value)
    {
        var property = typeof(T).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} に {propertyName} プロパティが見つかりません。");
        }

        property.SetValue(target, value);
    }
}