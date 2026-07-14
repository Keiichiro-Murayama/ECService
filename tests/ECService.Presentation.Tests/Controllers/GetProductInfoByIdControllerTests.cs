using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// 商品詳細取得コントローラの単体テスト。
/// </summary>
[TestClass]
public class GetProductInfoByIdControllerTests
{
    private Mock<IGetProductInfoUsecase> _getProductInfoUsecaseMock = null!;
    private GetProductViewModelAdapter _adapter = null!;
    private GetProductInfoByIdController _controller = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _getProductInfoUsecaseMock = new Mock<IGetProductInfoUsecase>();
        _adapter = new GetProductViewModelAdapter();

        _controller = new GetProductInfoByIdController(
            _getProductInfoUsecaseMock.Object,
            _adapter);
    }

    /// <summary>
    /// 商品が存在する場合、200 OKと商品詳細情報を返すこと。
    /// </summary>
    [TestMethod]
    public async Task GetInfoById_商品が存在する場合_Okと商品詳細情報を返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var product = CreateProduct(productUuid);

        _getProductInfoUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(productUuid))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetInfoById(productUuid);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

        var okResult = (OkObjectResult)result.Result!;
        Assert.AreEqual(200, okResult.StatusCode);

        Assert.IsInstanceOfType(okResult.Value, typeof(GetProductInfoResponse));

        var response = (GetProductInfoResponse)okResult.Value!;
        Assert.AreEqual(productUuid, response.ProductUuid);
        Assert.AreEqual("詳細取得テスト商品", response.ProductName);
        Assert.AreEqual(1200, response.Price);
        Assert.AreEqual(15, response.Stock);
        Assert.AreEqual(product.ProductCategory.CategoryUuid, response.CategoryId);
        Assert.AreEqual("https://example.com/product.png", response.ImageUrl);

        _getProductInfoUsecaseMock.Verify(
            usecase => usecase.ExecuteAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// 商品が存在しない場合、404 NotFoundを返すこと。
    /// </summary>
    [TestMethod]
    public async Task GetInfoById_商品が存在しない場合_NotFoundを返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _getProductInfoUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(productUuid))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetInfoById(productUuid);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));

        var notFoundResult = (NotFoundObjectResult)result.Result!;
        Assert.AreEqual(404, notFoundResult.StatusCode);

        _getProductInfoUsecaseMock.Verify(
            usecase => usecase.ExecuteAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// UsecaseでDomainExceptionが発生した場合、400 BadRequestを返すこと。
    /// </summary>
    [TestMethod]
    public async Task GetInfoById_UsecaseでDomainExceptionが発生した場合_BadRequestを返す()
    {
        // Arrange
        var productUuid = "invalid-uuid";

        _getProductInfoUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(productUuid))
            .ThrowsAsync(new DomainException(
                "商品UUIDの形式が不正です。",
                nameof(productUuid)));

        // Act
        var result = await _controller.GetInfoById(productUuid);

        // Assert
        Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));

        var badRequestResult = (BadRequestObjectResult)result.Result!;
        Assert.AreEqual(400, badRequestResult.StatusCode);

        _getProductInfoUsecaseMock.Verify(
            usecase => usecase.ExecuteAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// Usecaseで想定外の例外が発生した場合、例外がそのまま送出されること。
    /// </summary>
    [TestMethod]
    public async Task GetInfoById_Usecaseで想定外の例外が発生した場合_例外を送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _getProductInfoUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(productUuid))
            .ThrowsAsync(new InvalidOperationException("想定外エラー"));

        // Act
        var exception = await ThrowsAsync<InvalidOperationException>(
            () => _controller.GetInfoById(productUuid));

        // Assert
        Assert.AreEqual("想定外エラー", exception.Message);

        _getProductInfoUsecaseMock.Verify(
            usecase => usecase.ExecuteAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// テスト用の商品を生成する。
    /// </summary>
    private static Product CreateProduct(string productUuid)
    {
        var category = new ProductCategory(
            Guid.NewGuid().ToString(),
            "文房具");

        var stock = ProductStock.Restore(
            Guid.NewGuid().ToString(),
            15);

        return Product.Restore(
            productUuid,
            "詳細取得テスト商品",
            1200,
            "https://example.com/product.png",
            category,
            0,
            stock);
    }

    /// <summary>
    /// 指定した例外が送出されることを検証する。
    /// </summary>
    private static async Task<TException> ThrowsAsync<TException>(Func<Task> action)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException exception)
        {
            return exception;
        }
        catch (Exception exception)
        {
            Assert.Fail(
                $"想定した例外は {typeof(TException).Name} でしたが、実際は {exception.GetType().Name} でした。");
        }

        Assert.Fail($"想定した例外 {typeof(TException).Name} が送出されませんでした。");
        return null!;
    }
}