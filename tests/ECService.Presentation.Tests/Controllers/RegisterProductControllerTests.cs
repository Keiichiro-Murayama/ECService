using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using DomainException =
    ECService.Domain.Exceptions.DomainException;

using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Tests.Controllers;

[TestClass]
public partial class RegisterProductControllerTests
{
    private const string UploadedImageUrl =
        "https://example.com/photos/sample.png";

    private Mock<IRegisterProductUsecase>
        _usecaseMock = null!;

    private Mock<IProductImageStorage>
        _productImageStorageMock = null!; //石原:追加 商品画像保存処理のMock

    private Mock<
        ILogger<RegisterProductController>>
        _loggerMock = null!; //石原:追加 LoggerのMock

    private RegisterProductController
        _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock =
            new Mock<IRegisterProductUsecase>();

        _productImageStorageMock =
            new Mock<IProductImageStorage>();

        _loggerMock =
            new Mock<
                ILogger<RegisterProductController>>();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        _productImageStorageMock
            .Setup(
                storage =>
                    storage.UploadAsync(
                        It.IsAny<Stream>(),
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<
                            CancellationToken>()))
            .ReturnsAsync(
                UploadedImageUrl); //石原:追加 Blobへ保存した画像URLを返す

        _productImageStorageMock
            .Setup(
                storage =>
                    storage.DeleteAsync(
                        It.IsAny<string>(),
                        It.IsAny<
                            CancellationToken>()))
            .Returns(
                Task.CompletedTask); //石原:追加 登録失敗時の画像削除をMock化する

        _controller =
            new RegisterProductController(
                _usecaseMock.Object,
                new RegisterProductViewModelAdapter(),
                _productImageStorageMock.Object,
                _loggerMock.Object); //石原:変更 画像ストレージとLoggerを渡す

        _controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext(),
            }; //石原:追加 RequestAbortedを利用できるようHttpContextを設定する
    }

    /// <summary>
    /// テスト用の商品画像を生成する
    /// </summary>
    /// <param name="contentType">
    /// 画像のContent-Type
    /// </param>
    /// <param name="fileSize">
    /// ファイルサイズ
    /// </param>
    /// <returns>
    /// テスト用画像
    /// </returns>
    private static IFormFile CreateImageFile(
        string contentType = "image/png",
        int fileSize = 4)
    {
        byte[] imageBytes =
            new byte[fileSize];

        var imageStream =
            new MemoryStream(imageBytes);

        return new FormFile(
            imageStream,
            0,
            imageStream.Length,
            "Image",
            "sample.png")
        {
            Headers =
                new HeaderDictionary(),

            ContentType =
                contentType,
        };
    }

    /// <summary>
    /// 正常な商品登録リクエストを生成する
    /// </summary>
    /// <returns>
    /// 商品登録リクエスト
    /// </returns>
    private static RegisterProductRequest
        CreateValidRequest()
    {
        return new RegisterProductRequest
        {
            ProductName = "ボールペン",
            Price = 120,
            Stock = 50,
            CategoryUuid =
                Guid.NewGuid().ToString(),

            Image =
                CreateImageFile(), //石原:変更 ImageUrlではなく画像ファイルを設定する
        };
    }

    /// <summary>
    /// UT-REA-005
    /// 正常登録
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsCreated_WhenSuccess()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var response =
            (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status201Created,
            response.StatusCode);

        _productImageStorageMock.Verify(
            storage =>
                storage.UploadAsync(
                    It.IsAny<Stream>(),
                    request.ProductName,
                    request.Image!.ContentType,
                    It.IsAny<
                        CancellationToken>()),
            Times.Once);

        _usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<Product>()),
            Times.Once);
    }

    /// <summary>
    /// UT-REA-006
    /// 商品名未入力
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenProductNameIsEmpty()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.ProductName =
            string.Empty;

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    /// UT-REA-007
    /// 商品名1文字
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenNameLength1()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.ProductName = "A";

        _controller.ModelState.AddModelError(
            nameof(request.ProductName),
            "商品名は2文字以上");

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenNameLength21()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.ProductName =
            new string('あ', 21);

        _controller.ModelState.AddModelError(
            nameof(request.ProductName),
            "商品名は20文字以内で入力してください。");

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// 必須入力エラー
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenModelStateHasRequiredError()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        const string expectedMessage =
            "商品名を入力してください";

        _controller.ModelState.AddModelError(
            nameof(request.ProductName),
            expectedMessage);

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
            expectedMessage,
            GetResponseMessage(
                badRequest.Value));
    }

    /// <summary>
    /// UT-REA-009
    /// 価格未入力
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenPriceNull()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Price = null;

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenPriceOverLimit()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Price = 1_000_001;

        _controller.ModelState.AddModelError(
            nameof(request.Price),
            "価格は100万円以下で入力してください");

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenStockNull()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Stock = null;

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenStockOverLimit()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Stock = 1_001;

        _controller.ModelState.AddModelError(
            nameof(request.Stock),
            "在庫数は1000個以下で入力してください");

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenCategoryUuidIsEmpty()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.CategoryUuid =
            string.Empty;

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));
    }

    /// <summary>
    /// Guid形式不正
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenCategoryUuidIsInvalidFormat()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.CategoryUuid = "ABC";

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
            "カテゴリUUIDの形式が不正です。",
            GetResponseMessage(
                badRequest.Value));
    }

    /// <summary>
    /// UT-REA-014
    /// 商品画像未入力
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenImageIsNull()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Image = null; //石原:変更 ImageUrlではなく画像ファイルの未入力を確認する

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequest =
            (BadRequestObjectResult)result;

        Assert.AreEqual(
            "入力値に不備があります。",
            GetResponseMessage(
                badRequest.Value));
    }

    /// <summary>
    /// UT-REA-015
    /// 重複商品
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsConflict_WhenDuplicate()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .ThrowsAsync(
                new DomainException(
                    "既に登録されています"));

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ConflictObjectResult));

        var conflict =
            (ConflictObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflict.StatusCode);

        _productImageStorageMock.Verify(
            storage =>
                storage.DeleteAsync(
                    UploadedImageUrl,
                    It.IsAny<
                        CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// UT-REA-016
    /// DomainException
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenDomainExceptionOccurs()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .ThrowsAsync(
                new DomainException(
                    "入力エラー"));

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsBadRequest_WhenCategoryInternalExceptionOccurs()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .ThrowsAsync(
                new InternalException(
                    "カテゴリUUIDが存在しません"));

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

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
    public async Task
        RegisterProduct_ReturnsInternalServerError_WhenInternalExceptionOccurs()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .ThrowsAsync(
                new InternalException(
                    "DB Error"));

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var objectResult =
            (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            objectResult.StatusCode);
    }

    /// <summary>
    /// UT-REA-019
    /// 想定外Exception
    /// </summary>
    [TestMethod]
    public async Task
        RegisterProduct_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        _usecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync(
                        It.IsAny<Product>()))
            .ThrowsAsync(
                new Exception(
                    "Unexpected"));

        // Act
        IActionResult result =
            await _controller
                .RegisterProduct(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var objectResult =
            (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            objectResult.StatusCode);
    }

    /// <summary>
    /// 匿名オブジェクトからmessageを取得する
    /// </summary>
    /// <param name="responseValue">
    /// Controllerのレスポンス
    /// </param>
    /// <returns>
    /// メッセージ
    /// </returns>
    private static string? GetResponseMessage(
        object? responseValue)
    {
        return responseValue?
            .GetType()
            .GetProperty("message")?
            .GetValue(responseValue)?
            .ToString();
    }
}