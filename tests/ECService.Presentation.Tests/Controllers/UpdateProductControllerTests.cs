using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// 商品修正コントローラの単体テスト。
/// </summary>
[TestClass]
public class UpdateProductControllerTests
{
    private Mock<IUpdateProductUsecase> _updateProductUsecaseMock = null!;
    private UpdateProductController _controller = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _updateProductUsecaseMock = new Mock<IUpdateProductUsecase>();
        _controller = new UpdateProductController(_updateProductUsecaseMock.Object);
    }

    /// <summary>
    /// 正常なリクエストの場合、200 OKを返すこと。
    /// </summary>
    [TestMethod]
    public async Task UpdateProduct_正常なリクエストの場合_Okを返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var request = CreateRequest();
        var product = CreateProduct(productUuid, request);

        _updateProductUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                productUuid,
                request.ProductName,
                request.Price,
                request.Stock,
                request.CategoryId,
                request.ImageUrl))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.UpdateProduct(productUuid, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkObjectResult));

        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            productUuid,
            request.ProductName,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.ImageUrl), Times.Once);
    }

    /// <summary>
    /// ModelStateが不正な場合、400 BadRequestを返すこと。
    /// </summary>
    [TestMethod]
    public async Task UpdateProduct_ModelStateが不正な場合_BadRequestを返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var request = CreateRequest();

        _controller.ModelState.AddModelError(
            nameof(UpdateProductRequest.ProductName),
            "商品名を入力してください");

        // Act
        var result = await _controller.UpdateProduct(productUuid, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequestResult = (BadRequestObjectResult)result;
        Assert.AreEqual(400, badRequestResult.StatusCode);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    /// UsecaseでDomainExceptionが発生した場合、400 BadRequestを返すこと。
    /// </summary>
    [TestMethod]
    public async Task UpdateProduct_UsecaseでDomainExceptionが発生した場合_BadRequestを返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var request = CreateRequest();

        _updateProductUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                productUuid,
                request.ProductName,
                request.Price,
                request.Stock,
                request.CategoryId,
                request.ImageUrl))
            .ThrowsAsync(new DomainException(
                "商品が見つかりません。",
                nameof(productUuid)));

        // Act
        var result = await _controller.UpdateProduct(productUuid, request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequestResult = (BadRequestObjectResult)result;
        Assert.AreEqual(400, badRequestResult.StatusCode);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            productUuid,
            request.ProductName,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.ImageUrl), Times.Once);
    }

    /// <summary>
    /// Usecaseで想定外の例外が発生した場合、例外がそのまま送出されること。
    /// </summary>
    [TestMethod]
    public async Task UpdateProduct_Usecaseで想定外の例外が発生した場合_例外を送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var request = CreateRequest();

        _updateProductUsecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                productUuid,
                request.ProductName,
                request.Price,
                request.Stock,
                request.CategoryId,
                request.ImageUrl))
            .ThrowsAsync(new InvalidOperationException("想定外エラー"));

        // Act
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
            _controller.UpdateProduct(productUuid, request));

        // Assert
        Assert.AreEqual("想定外エラー", exception.Message);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            productUuid,
            request.ProductName,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.ImageUrl), Times.Once);
    }

    /// <summary>
    /// テスト用の商品修正リクエストを生成する。
    /// </summary>
    private static UpdateProductRequest CreateRequest()
    {
        return new UpdateProductRequest
        {
            ProductName = "修正商品",
            Price = 1500,
            Stock = 20,
            CategoryId = Guid.NewGuid().ToString(),
            ImageUrl = "https://example.com/update.png"
        };
    }

    /// <summary>
    /// テスト用の商品を生成する。
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        UpdateProductRequest request)
    {
        var category = new ProductCategory(
            request.CategoryId,
            "文房具");

        return Product.Restore(
            productUuid,
            request.ProductName,
            request.Price,
            request.ImageUrl,
            category,
            0,
            ProductStock.Restore(Guid.NewGuid().ToString(), request.Stock));
    }
}