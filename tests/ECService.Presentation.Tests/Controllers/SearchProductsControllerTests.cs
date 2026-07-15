using System.Reflection;
using System.Runtime.CompilerServices;
using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// SearchProductsController の単体テスト
///
/// 商品検索APIにおいて、ControllerがUsecaseを呼び出し、
/// READMEの仕様どおり、商品一覧の配列・404・500レスポンスを返すことを検証する。
/// </summary>
[TestClass]
public class SearchProductsControllerTests
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
    /// カテゴリ指定なしで検索した場合、200 OK と商品一覧が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryIsNull_ReturnsOkObjectResult()
    {
        // Arrange
        Log("Search_CategoryIsNull_ReturnsOkObjectResult：テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
                name: "エコバッグ",
                price: 880,
                imageUrl: "https://example.com/images/bag.jpg")
        };

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(null))
            .ReturnsAsync(products);

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(null);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.HasCount(1, response);
        Assert.AreEqual("b7af7239-108b-4698-b2a7-2fe4469b275a", response[0].ProductUuid);
        Assert.AreEqual("エコバッグ", response[0].ProductName);
        Assert.AreEqual(880, response[0].Price);
        Assert.AreEqual("https://example.com/images/bag.jpg", response[0].ImageUrl);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(null),
            Times.Once);

        Log("カテゴリ指定なしの場合、200 OK と商品一覧の配列が返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリ指定ありで検索した場合、200 OK と商品一覧が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryExists_ReturnsOkObjectResult()
    {
        // Arrange
        Log("Search_CategoryExists_ReturnsOkObjectResult：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "eb07baff-7f28-4356-abfb-020c31e04dc7",
                name: "Type-Cハブ",
                price: 3980,
                imageUrl: "https://example.com/images/hub.jpg")
        };

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(products);

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.HasCount(1, response);
        Assert.AreEqual("eb07baff-7f28-4356-abfb-020c31e04dc7", response[0].ProductUuid);
        Assert.AreEqual("Type-Cハブ", response[0].ProductName);
        Assert.AreEqual(3980, response[0].Price);
        Assert.AreEqual("https://example.com/images/hub.jpg", response[0].ImageUrl);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("カテゴリ指定ありの場合、200 OK と商品一覧の配列が返ることを確認しました。");
    }

    /// <summary>
    /// 検索結果が0件の場合、200 OK と空の配列が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_ResultIsEmpty_ReturnsEmptyProducts()
    {
        // Arrange
        Log("Search_ResultIsEmpty_ReturnsEmptyProducts：テスト開始");

        var categoryUuid = "11111111-1111-1111-1111-111111111111";

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var okResult = actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response = okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.IsEmpty(response);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("検索結果が0件の場合でも、200 OK と空の配列が返ることを確認しました。");
    }

    /// <summary>
    /// ControllerからUsecaseへカテゴリUUIDが正しく渡されること
    /// </summary>
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

        var controller = CreateController(usecaseMock);

        // Act
        await controller.Search(categoryUuid);

        // Assert
        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("ControllerからUsecaseへ categoryUuid が正しく渡されることを確認しました。");
    }

    /// <summary>
    /// DomainException が発生した場合、404 NotFound が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_UsecaseThrowsDomainException_ReturnsNotFound()
    {
        // Arrange
        Log("Search_UsecaseThrowsDomainException_ReturnsNotFound：テスト開始");

        var categoryUuid = "99999999-9999-9999-9999-999999999999";

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ThrowsAsync(new DomainException(
                "指定されたカテゴリID（UUID）が存在しません。",
                nameof(categoryUuid)));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            GetMessage(notFoundResult.Value));

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("DomainException が発生した場合、404 NotFound が返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリ/UUID関連のInternalExceptionが発生した場合、404 NotFound が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_InternalExceptionAboutCategory_ReturnsNotFound()
    {
        // Arrange
        Log("Search_InternalExceptionAboutCategory_ReturnsNotFound：テスト開始");

        var categoryUuid = "99999999-9999-9999-9999-999999999999";

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(categoryUuid))
            .ThrowsAsync(new InternalException("カテゴリUUIDが存在しません。"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(categoryUuid);

        // Assert
        var notFoundResult = actionResult.Result as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            GetMessage(notFoundResult.Value));

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log("カテゴリ/UUID関連の InternalException の場合、404 NotFound が返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリ/UUID以外のInternalExceptionが発生した場合、500 Internal Server Error が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_InternalExceptionOtherReason_ReturnsInternalServerError()
    {
        // Arrange
        Log("Search_InternalExceptionOtherReason_ReturnsInternalServerError：テスト開始");

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(null))
            .ThrowsAsync(new InternalException("DB接続エラーが発生しました。"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(null);

        // Assert
        var statusCodeResult = actionResult.Result as ObjectResult;

        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
        Assert.AreEqual(
            "InternalException: サーバー内部で予期せぬエラーが発生しました。",
            GetMessage(statusCodeResult.Value));

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(null),
            Times.Once);

        Log("カテゴリ/UUID以外の InternalException の場合、500 が返ることを確認しました。");
    }

    /// <summary>
    /// 想定外のExceptionが発生した場合、500 Internal Server Error が返ること
    /// </summary>
    [TestMethod]
    public async Task Search_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        Log("Search_UnexpectedException_ReturnsInternalServerError：テスト開始");

        var usecaseMock = new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(null))
            .ThrowsAsync(new Exception("想定外のエラー"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult = await controller.Search(null);

        // Assert
        var statusCodeResult = actionResult.Result as ObjectResult;

        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(500, statusCodeResult.StatusCode);
        Assert.AreEqual(
            "InternalException: サーバー内部で予期せぬエラーが発生しました。",
            GetMessage(statusCodeResult.Value));

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(null),
            Times.Once);

        Log("想定外の Exception の場合、500 が返ることを確認しました。");
    }

    /// <summary>
    /// テスト用のControllerを作成する
    /// </summary>
    private static SearchProductsController CreateController(
        Mock<ISearchProductsUsecase> usecaseMock)
    {
        var adapter = new SearchProductsViewModelAdapter();

        return new SearchProductsController(
            usecaseMock.Object,
            adapter);
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

    /// <summary>
    /// 匿名オブジェクトの message プロパティを取得する
    /// </summary>
    private static string? GetMessage(object? response)
    {
        if (response is null)
        {
            return null;
        }

        var property = response.GetType().GetProperty("message");

        return property?.GetValue(response)?.ToString();
    }
}