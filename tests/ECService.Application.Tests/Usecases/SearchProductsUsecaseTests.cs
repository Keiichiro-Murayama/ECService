using System.Reflection;
using System.Runtime.Serialization;
using ECService.Application.Usecases.Imps;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// SearchProductsUsecase の単体テスト
///
/// 商品検索機能において、カテゴリUUIDの有無により
/// 全商品検索・カテゴリ検索が正しく呼び分けられることを検証する。
/// </summary>
[TestClass]
public class SearchProductsUsecaseTests
{
    public TestContext TestContext { get; set; } = null!;

    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsNull_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsNull_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "エコバッグ",
                880,
                "https://example.com/images/bag.jpg")
        };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("エコバッグ", result[0].Name);

        repositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Once);
        repositoryMock.Verify(repository => repository.SelectByCategoryAsync(It.IsAny<string>()), Times.Never);

        Log("categoryUuid が null の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsEmpty_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsEmpty_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                "耐水ノート",
                450,
                "https://example.com/images/note.jpg")
        };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync("");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("耐水ノート", result[0].Name);

        repositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Once);
        repositoryMock.Verify(repository => repository.SelectByCategoryAsync(It.IsAny<string>()), Times.Never);

        Log("categoryUuid が空文字の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidIsWhiteSpace_CallsSelectAllAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidIsWhiteSpace_CallsSelectAllAsync：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                "45c24c9f-a494-4e75-afb8-794c5c66135f",
                "充電式マウス",
                2480,
                "https://example.com/images/mouse.jpg")
        };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync("   ");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("充電式マウス", result[0].Name);

        repositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Once);
        repositoryMock.Verify(repository => repository.SelectByCategoryAsync(It.IsAny<string>()), Times.Never);

        Log("categoryUuid が空白の場合、SelectAllAsync が1回呼ばれることを確認しました。");
    }

    [TestMethod]
    public async Task ExecuteAsync_CategoryUuidExists_CallsSelectByCategoryAsync()
    {
        // Arrange
        Log("ExecuteAsync_CategoryUuidExists_CallsSelectByCategoryAsync：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var products = new List<Product>
        {
            CreateProduct(
                "eb07baff-7f28-4356-abfb-020c31e04dc7",
                "Type-Cハブ",
                3980,
                "https://example.com/images/hub.jpg")
        };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
            .ReturnsAsync(products);

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Type-Cハブ", result[0].Name);

        repositoryMock.Verify(repository => repository.SelectByCategoryAsync(categoryUuid), Times.Once);
        repositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Never);

        Log("categoryUuid が指定された場合、SelectByCategoryAsync が1回呼ばれることを確認しました。");
    }

    [TestMethod]
    public async Task ExecuteAsync_SelectAllReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        Log("ExecuteAsync_SelectAllReturnsEmptyList_ReturnsEmptyList：テスト開始");

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<Product>());

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        repositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Once);

        Log("全商品検索結果が0件の場合、空のリストが返ることを確認しました。");
    }

    [TestMethod]
    public async Task ExecuteAsync_SelectByCategoryReturnsEmptyList_ReturnsEmptyList()
    {
        // Arrange
        Log("ExecuteAsync_SelectByCategoryReturnsEmptyList_ReturnsEmptyList：テスト開始");

        var categoryUuid = "99999999-9999-9999-9999-999999999999";

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var usecase = new SearchProductsUsecase(repositoryMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(categoryUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        repositoryMock.Verify(repository => repository.SelectByCategoryAsync(categoryUuid), Times.Once);

        Log("カテゴリ検索結果が0件の場合、空のリストが返ることを確認しました。");
    }

    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product =
            (Product)FormatterServices.GetUninitializedObject(typeof(Product));

        SetProperty(product, nameof(Product.ProductUuid), productUuid);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.ImageUrl), imageUrl);

        return product;
    }

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