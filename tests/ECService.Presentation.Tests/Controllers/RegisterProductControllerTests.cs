using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Tests.Controllers;

[TestClass]
public partial class RegisterProductControllerTests
{
    private Mock<IRegisterProductUsecase> _usecaseMock = null!;
    private RegisterProductController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock = new Mock<IRegisterProductUsecase>();

        _controller = new RegisterProductController(
            _usecaseMock.Object,
            new RegisterProductViewModelAdapter());
    }

    private RegisterProductRequest CreateValidRequest()
    {
        return new RegisterProductRequest
        {
            ProductName = "ボールペン",
            Price = 120,
            Stock = 50,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "http://image.com/sample.png"
        };
    }

    /// <summary>
    /// UT-REA-005
    /// 正常登録
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsCreated_WhenSuccess()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));

        var response = (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status201Created,
            response.StatusCode);

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<Product>()),
            Times.Once);
    }

    /// <summary>
    /// UT-REA-006
    /// Requestがnullの場合
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenRequestIsNull()
    {
        // Act
        var result = await _controller.RegisterProduct(null!);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);

        Assert.AreEqual(
            "productName、price、stock、categoryUuidを入力してください。",
            badRequest.Value!
                .GetType()
                .GetProperty("message")!
                .GetValue(badRequest.Value));
    }

    /// <summary>
    /// UT-REA-007
    /// 商品名1文字
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenNameLength1()
    {
        // Arrange
        var request = CreateValidRequest();

        request.ProductName = "A";

        _controller.ModelState.AddModelError(
            "ProductName",
            "商品名は2文字以上");

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// UT-REA-008
    /// 商品名21文字
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenNameLength21()
    {
        // Arrange
        var request = CreateValidRequest();

        request.ProductName = new string('あ', 21);

        _controller.ModelState.AddModelError(
            "ProductName",
            "商品名は20文字以内で入力してください。");

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// 必須入力エラー
    /// HasRequiredError()を通す
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenModelStateHasRequiredError()
    {
        // Arrange
        var request = CreateValidRequest();

        _controller.ModelState.AddModelError(
            "ProductName",
            "商品名を入力してください");

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);

        Assert.AreEqual(
            "productName、price、stock、categoryUuidを入力してください。",
            badRequest.Value!
                .GetType()
                .GetProperty("message")!
                .GetValue(badRequest.Value));
    }

    /// <summary>
    /// UT-REA-009
    /// 価格未入力
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenPriceNull()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Price = null;

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// UT-REA-010
    /// 価格上限超過
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenPriceOverLimit()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Price = 1000001;

        _controller.ModelState.AddModelError(
            "Price",
            "価格は100万円以下で入力してください");

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// UT-REA-011
    /// 在庫未入力
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenStockNull()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Stock = null;

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// UT-REA-012
    /// 在庫上限超過
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenStockOverLimit()
    {
        // Arrange
        var request = CreateValidRequest();

        request.Stock = 1001;

        _controller.ModelState.AddModelError(
            "Stock",
            "在庫数は1000個以下で入力してください");

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }
 /// <summary>
    /// UT-REA-013
    /// カテゴリ未入力
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenCategoryUuidIsEmpty()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CategoryUuid = "";

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Guid形式不正
    /// Controller95～99行のカバレッジ
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenCategoryUuidIsInvalidFormat()
    {
        // Arrange
        var request = CreateValidRequest();
        request.CategoryUuid = "ABC";

        // Act
        var result = await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequest =
            (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);

        Assert.AreEqual(
            "入力値に不備があります。",
            badRequest.Value!
                .GetType()
                .GetProperty("message")!
                .GetValue(badRequest.Value));
    }

    /// <summary>
    /// UT-REA-014
    /// 画像URL未入力
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenImageUrlIsEmpty()
    {
        // Arrange
        var request = CreateValidRequest();
        request.ImageUrl = "";

        _controller.ModelState.AddModelError(
            "ImageUrl",
            "画像をアップロードしてください");

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// UT-REA-015
    /// 重複商品
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsConflict_WhenDuplicate()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Product>()))
            .ThrowsAsync(
                new DomainException("既に登録されています"));

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ConflictObjectResult));

        var conflict =
            (ConflictObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflict.StatusCode);
    }

    /// <summary>
    /// UT-REA-016
    /// DomainException
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenDomainExceptionOccurs()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Product>()))
            .ThrowsAsync(
                new DomainException("入力エラー"));

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequest =
            (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);
    }

    /// <summary>
    /// UT-REA-017
    /// InternalException（カテゴリ）
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsBadRequest_WhenCategoryInternalExceptionOccurs()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Product>()))
            .ThrowsAsync(
                new InternalException("カテゴリUUIDが存在しません"));

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequest =
            (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);
    }

    /// <summary>
    /// UT-REA-018
    /// InternalException（その他）
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsInternalServerError_WhenInternalExceptionOccurs()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Product>()))
            .ThrowsAsync(
                new InternalException("DB Error"));

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var objectResult =
            (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);
    }

    /// <summary>
    /// UT-REA-019
    /// 想定外Exception
    /// </summary>
    [TestMethod]
    public async Task RegisterProduct_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<Product>()))
            .ThrowsAsync(
                new Exception("Unexpected"));

        // Act
        var result =
            await _controller.RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var objectResult =
            (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);
    }
}
