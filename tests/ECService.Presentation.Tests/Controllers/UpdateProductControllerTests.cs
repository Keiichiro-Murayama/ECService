using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// 商品修正コントローラの単体テスト。
/// </summary>
public class UpdateProductControllerTests
{
    private readonly Mock<IUpdateProductUsecase> _updateProductUsecaseMock;
    private readonly UpdateProductController _controller;

    public UpdateProductControllerTests()
    {
        _updateProductUsecaseMock = new Mock<IUpdateProductUsecase>();
        _controller = new UpdateProductController(_updateProductUsecaseMock.Object);
    }

    [Fact]
    public async Task UpdateProduct_正常なリクエストの場合_Okを返す()
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
            .ReturnsAsync(default(ECService.Domain.Models.Product)!);

        // Act
        var result = await _controller.UpdateProduct(productUuid, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            productUuid,
            request.ProductName,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.ImageUrl), Times.Once);
    }

    [Fact]
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
        Assert.IsType<BadRequestObjectResult>(result);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
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
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);

        _updateProductUsecaseMock.Verify(usecase => usecase.ExecuteAsync(
            productUuid,
            request.ProductName,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.ImageUrl), Times.Once);
    }

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
}