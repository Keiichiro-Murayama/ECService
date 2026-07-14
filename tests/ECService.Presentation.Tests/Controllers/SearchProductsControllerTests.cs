using System.Reflection;
using System.Runtime.Serialization;
using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// SearchProductsController の単体テスト
///
/// 商品検索APIにおいて、ControllerがUsecaseを呼び出し、
/// 正しいレスポンスを返すことを検証する。
/// </summary>
[TestClass]
public class SearchProductsControllerTests
{
    public TestContext TestContext { get; set; } = null!;

    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    [TestMethod]
    public async Task Search_CategoryIsNull_ReturnsOkObjectResult()
    {
        // Arrange
        Log("Search_CategoryIsNull_ReturnsOkObjectResult：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "エコバッグ",
                880,
                "https://example.com/images/bag.jpg")
        };

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(null))
            .ReturnsAsync(products);

        var adapter = new SearchProductsViewModelAdapter();

        var controller = new SearchProductsController(
            usecaseMock.Object,
            adapter);

        // Act
        var actionResult = await controller.Search(null);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as SearchProductsResponse;

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Products.Count);
        Assert.AreEqual("エコバッグ", response.Products[0].ProductName);

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(null), Times.Once);

        Log("カテゴリ指定なしの場合、200 OK と商品検索レスポンスが返ることを確認しました。");
    }

    [TestMethod]
    public async Task Search_CategoryExists_ReturnsOkObjectResult()
    {
        // Arrange
        Log("Search_CategoryExists_ReturnsOkObjectResult：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var products = new List<Product>
        {
            CreateProduct(
                "eb07baff-7f28-4356-abfb-020c31e04dc7",
                "Type-Cハブ",
                3980,
                "https://example.com/images/hub.jpg")
        };

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(products);

        var adapter = new SearchProductsViewModelAdapter();

        var controller = new SearchProductsController(
            usecaseMock.Object,
            adapter);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as SearchProductsResponse;

        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Products.Count);
        Assert.AreEqual("Type-Cハブ", response.Products[0].ProductName);

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(categoryUuid), Times.Once);

        Log("カテゴリ指定ありの場合、200 OK と商品検索レスポンスが返ることを確認しました。");
    }

    [TestMethod]
    public async Task Search_ResultIsEmpty_ReturnsEmptyProducts()
    {
        // Arrange
        Log("Search_ResultIsEmpty_ReturnsEmptyProducts：テスト開始");

        var categoryUuid = "99999999-9999-9999-9999-999999999999";

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var adapter = new SearchProductsViewModelAdapter();

        var controller = new SearchProductsController(
            usecaseMock.Object,
            adapter);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as SearchProductsResponse;

        Assert.IsNotNull(response);
        Assert.AreEqual(0, response.Products.Count);

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(categoryUuid), Times.Once);

        Log("検索結果が0件の場合でも、空の products を持つレスポンスが返ることを確認しました。");
    }

    [TestMethod]
    public async Task Search_CategoryUuid_IsPassedToUsecase()
    {
        // Arrange
        Log("Search_CategoryUuid_IsPassedToUsecase：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var adapter = new SearchProductsViewModelAdapter();

        var controller = new SearchProductsController(
            usecaseMock.Object,
            adapter);

        // Act
        await controller.Search(categoryUuid);

        // Assert
        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("ControllerからUsecaseへカテゴリUUIDが正しく渡されることを確認しました。");
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