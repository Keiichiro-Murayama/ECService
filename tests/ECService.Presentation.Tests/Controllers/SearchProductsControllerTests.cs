using System.Reflection;
using System.Runtime.CompilerServices;
using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DomainException =
    ECService.Domain.Exceptions.DomainException;
using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// SearchProductsControllerの単体テスト。
///
/// 商品検索APIにおいて、ControllerがUsecaseを呼び出し、
/// 商品一覧、400、404、500のレスポンスを返すことを検証する。
/// また、ControllerにAuthorize属性が設定されていることを検証する。
/// </summary>
[TestClass]
public class SearchProductsControllerTests
{
    /// <summary>
    /// テスト出力用。
    /// </summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// ターミナルとテスト結果にログを出力する。
    /// </summary>
    /// <param name="message">出力するメッセージ。</param>
    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    /// <summary>
    /// SearchProductsControllerにAuthorize属性が
    /// 設定されていること。
    /// </summary>
    [TestMethod]
    public void SearchProductsController_HasAuthorizeAttribute()
    {
        // Arrange
        Log(
            "SearchProductsController_HasAuthorizeAttribute："
            + "テスト開始");

        // Act
        var authorizeAttribute =
            typeof(SearchProductsController)
                .GetCustomAttribute<AuthorizeAttribute>();

        // Assert
        Assert.IsNotNull(authorizeAttribute);

        Log(
            "SearchProductsControllerにAuthorize属性が"
            + "設定されていることを確認しました。");
    }

    /// <summary>
    /// カテゴリ指定なしで検索した場合、
    /// 200 OKと商品一覧が返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryIsNull_ReturnsOkObjectResult()
    {
        // Arrange
        Log(
            "Search_CategoryIsNull_ReturnsOkObjectResult："
            + "テスト開始");

        var products = new List<Product>
        {
            CreateProduct(
                productUuid:
                    "b7af7239-108b-4698-b2a7-2fe4469b275a",
                name: "エコバッグ",
                price: 880,
                imageUrl:
                    "https://example.com/images/bag.jpg")
        };

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(null))
            .ReturnsAsync(products);

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(null);

        // Assert
        var okResult =
            actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        var response =
            okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.HasCount(1, response);

        Assert.AreEqual(
            "b7af7239-108b-4698-b2a7-2fe4469b275a",
            response[0].ProductUuid);

        Assert.AreEqual(
            "エコバッグ",
            response[0].ProductName);

        Assert.AreEqual(
            880,
            response[0].Price);

        Assert.AreEqual(
            "https://example.com/images/bag.jpg",
            response[0].ImageUrl);

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(null),
            Times.Once);

        Log(
            "カテゴリ指定なしの場合、200 OKと"
            + "商品一覧の配列が返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリ指定ありで検索した場合、
    /// 200 OKと商品一覧が返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryExists_ReturnsOkObjectResult()
    {
        // Arrange
        Log(
            "Search_CategoryExists_ReturnsOkObjectResult："
            + "テスト開始");

        var categoryUuid =
            "11111111-1111-1111-1111-111111111111";

        var products = new List<Product>
        {
            CreateProduct(
                productUuid:
                    "eb07baff-7f28-4356-abfb-020c31e04dc7",
                name: "Type-Cハブ",
                price: 3980,
                imageUrl:
                    "https://example.com/images/hub.jpg")
        };

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(products);

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(categoryUuid);

        // Assert
        var okResult =
            actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        var response =
            okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.HasCount(1, response);

        Assert.AreEqual(
            "eb07baff-7f28-4356-abfb-020c31e04dc7",
            response[0].ProductUuid);

        Assert.AreEqual(
            "Type-Cハブ",
            response[0].ProductName);

        Assert.AreEqual(
            3980,
            response[0].Price);

        Assert.AreEqual(
            "https://example.com/images/hub.jpg",
            response[0].ImageUrl);

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log(
            "カテゴリ指定ありの場合、200 OKと"
            + "商品一覧の配列が返ることを確認しました。");
    }

    /// <summary>
    /// 検索結果が0件の場合、
    /// 200 OKと空の配列が返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_ResultIsEmpty_ReturnsEmptyProducts()
    {
        // Arrange
        Log(
            "Search_ResultIsEmpty_ReturnsEmptyProducts："
            + "テスト開始");

        var categoryUuid =
            "11111111-1111-1111-1111-111111111111";

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(categoryUuid);

        // Assert
        var okResult =
            actionResult.Result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        var response =
            okResult.Value as List<ProductsItem>;

        Assert.IsNotNull(response);
        Assert.IsEmpty(response);

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log(
            "検索結果が0件の場合でも、200 OKと"
            + "空の配列が返ることを確認しました。");
    }

    /// <summary>
    /// ControllerからUsecaseへカテゴリUUIDが
    /// 正しく渡されること。
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryUuid_IsPassedToUsecase()
    {
        // Arrange
        Log(
            "Search_CategoryUuid_IsPassedToUsecase："
            + "テスト開始");

        var categoryUuid =
            "11111111-1111-1111-1111-111111111111";

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(categoryUuid))
            .ReturnsAsync(new List<Product>());

        var controller = CreateController(usecaseMock);

        // Act
        await controller.Search(categoryUuid);

        // Assert
        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log(
            "ControllerからUsecaseへcategoryUuidが"
            + "正しく渡されることを確認しました。");
    }

    /// <summary>
    /// categoryUuidがUUID形式ではない場合、
    /// 400 Bad Requestが返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_CategoryUuidIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        Log(
            "Search_CategoryUuidIsInvalid_ReturnsBadRequest："
            + "テスト開始");

        var invalidCategoryUuid = "abc";

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(invalidCategoryUuid);

        // Assert
        var badRequestResult =
            actionResult.Result as BadRequestObjectResult;

        Assert.IsNotNull(badRequestResult);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.AreEqual(
            "カテゴリUUIDの形式が正しくありません。",
            GetMessage(badRequestResult.Value));

        // UUID形式チェックで処理が終了するため、
        // Usecaseが呼び出されないことを確認する
        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<string?>()),
            Times.Never);

        Log(
            "UUID形式ではないcategoryUuidを指定した場合、"
            + "400 Bad Requestが返ることを確認しました。");
    }

    /// <summary>
    /// DomainExceptionが発生した場合、
    /// 404 Not Foundが返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_UsecaseThrowsDomainException_ReturnsNotFound()
    {
        // Arrange
        Log(
            "Search_UsecaseThrowsDomainException_ReturnsNotFound："
            + "テスト開始");

        var categoryUuid =
            "99999999-9999-9999-9999-999999999999";

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(categoryUuid))
            .ThrowsAsync(
                new DomainException(
                    "指定されたカテゴリID（UUID）が存在しません。",
                    nameof(categoryUuid)));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(categoryUuid);

        // Assert
        var notFoundResult =
            actionResult.Result as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);

        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            notFoundResult.StatusCode);

        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            GetMessage(notFoundResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log(
            "DomainExceptionが発生した場合、"
            + "404 Not Foundが返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリまたはUUIDに関するInternalExceptionが
    /// 発生した場合、404 Not Foundが返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_InternalExceptionAboutCategory_ReturnsNotFound()
    {
        // Arrange
        Log(
            "Search_InternalExceptionAboutCategory_ReturnsNotFound："
            + "テスト開始");

        var categoryUuid =
            "99999999-9999-9999-9999-999999999999";

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(categoryUuid))
            .ThrowsAsync(
                new InternalException(
                    "カテゴリUUIDが存在しません。"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(categoryUuid);

        // Assert
        var notFoundResult =
            actionResult.Result as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);

        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            notFoundResult.StatusCode);

        Assert.AreEqual(
            "指定されたカテゴリID（UUID）が存在しません。",
            GetMessage(notFoundResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(categoryUuid),
            Times.Once);

        Log(
            "カテゴリまたはUUIDに関するInternalExceptionの場合、"
            + "404 Not Foundが返ることを確認しました。");
    }

    /// <summary>
    /// カテゴリまたはUUID以外のInternalExceptionが
    /// 発生した場合、500 Internal Server Errorが返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_InternalExceptionOtherReason_ReturnsInternalServerError()
    {
        // Arrange
        Log(
            "Search_InternalExceptionOtherReason_"
            + "ReturnsInternalServerError：テスト開始");

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(null))
            .ThrowsAsync(
                new InternalException(
                    "DB接続エラーが発生しました。"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(null);

        // Assert
        var statusCodeResult =
            actionResult.Result as ObjectResult;

        Assert.IsNotNull(statusCodeResult);

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            statusCodeResult.StatusCode);

        Assert.AreEqual(
            "InternalException: サーバー内部で予期せぬエラーが発生しました。",
            GetMessage(statusCodeResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(null),
            Times.Once);

        Log(
            "カテゴリまたはUUID以外のInternalExceptionの場合、"
            + "500 Internal Server Errorが返ることを確認しました。");
    }

    /// <summary>
    /// 想定外のExceptionが発生した場合、
    /// 500 Internal Server Errorが返ること。
    /// </summary>
    [TestMethod]
    public async Task Search_UnexpectedException_ReturnsInternalServerError()
    {
        // Arrange
        Log(
            "Search_UnexpectedException_"
            + "ReturnsInternalServerError：テスト開始");

        var usecaseMock =
            new Mock<ISearchProductsUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(null))
            .ThrowsAsync(
                new Exception(
                    "想定外のエラー"));

        var controller = CreateController(usecaseMock);

        // Act
        var actionResult =
            await controller.Search(null);

        // Assert
        var statusCodeResult =
            actionResult.Result as ObjectResult;

        Assert.IsNotNull(statusCodeResult);

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            statusCodeResult.StatusCode);

        Assert.AreEqual(
            "InternalException: サーバー内部で予期せぬエラーが発生しました。",
            GetMessage(statusCodeResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(null),
            Times.Once);

        Log(
            "想定外のExceptionの場合、"
            + "500 Internal Server Errorが返ることを確認しました。");
    }

    /// <summary>
    /// テスト用のControllerを作成する。
    /// </summary>
    /// <param name="usecaseMock">
    /// 商品検索ユースケースのモック。
    /// </param>
    /// <returns>テスト対象のController。</returns>
    private static SearchProductsController CreateController(
        Mock<ISearchProductsUsecase> usecaseMock)
    {
        var adapter =
            new SearchProductsViewModelAdapter();

        return new SearchProductsController(
            usecaseMock.Object,
            adapter);
    }

    /// <summary>
    /// テスト用の商品を作成する。
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product =
            (Product)RuntimeHelpers.GetUninitializedObject(
                typeof(Product));

        SetProperty(
            product,
            nameof(Product.ProductUuid),
            productUuid);

        SetProperty(
            product,
            nameof(Product.Name),
            name);

        SetProperty(
            product,
            nameof(Product.Price),
            price);

        SetProperty(
            product,
            nameof(Product.ImageUrl),
            imageUrl);

        return product;
    }

    /// <summary>
    /// private setのプロパティに
    /// テスト用の値を設定する。
    /// </summary>
    private static void SetProperty<T>(
        T target,
        string propertyName,
        object value)
    {
        var property =
            typeof(T).GetProperty(
                propertyName,
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name}に"
                + $"{propertyName}プロパティが見つかりません。");
        }

        property.SetValue(target, value);
    }

    /// <summary>
    /// 匿名オブジェクトのmessageプロパティを取得する。
    /// </summary>
    private static string? GetMessage(object? response)
    {
        if (response is null)
        {
            return null;
        }

        var property =
            response.GetType().GetProperty("message");

        return property?
            .GetValue(response)?
            .ToString();
    }
}