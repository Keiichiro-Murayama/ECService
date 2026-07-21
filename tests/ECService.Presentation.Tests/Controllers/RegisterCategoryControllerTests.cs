using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

[TestClass]
public class RegisterCategoryControllerTests
{
    private Mock<IRegisterProductCategoryUsecase> _usecaseMock = null!;
    private RegisterCategoryController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock = new Mock<IRegisterProductCategoryUsecase>();

        _controller = new RegisterCategoryController(
            _usecaseMock.Object,
            new RegisterCategoryViewModelAdapter());
    }


    /// <summary>
    /// UT-REC-006
    /// カテゴリ登録が成功すること
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsCreated_WhenValidRequest()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "文房具"
        };

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ObjectResult));

        var objectResult = (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status201Created,
            objectResult.StatusCode);


        var response = objectResult.Value!;

        var uuid = response
            .GetType()
            .GetProperty("categoryUuid")?
            .GetValue(response)?
            .ToString();

        var message = response
            .GetType()
            .GetProperty("message")?
            .GetValue(response)?
            .ToString();


        Assert.IsFalse(string.IsNullOrEmpty(uuid));

        Assert.AreEqual(
            "商品カテゴリを登録しました。",
            message);


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Once);
    }


    /// <summary>
    /// UT-REC-007
    /// カテゴリ名重複の場合409を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsConflict_WhenCategoryAlreadyExists()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "文房具"
        };


        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(
                new DomainException(
                    "カテゴリ名:文房具は既に存在します。"));


        // Act
        var result = await _controller.Register(request);


        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(ConflictObjectResult));


        var conflict = (ConflictObjectResult)result;


        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflict.StatusCode);


        var message = conflict.Value!
            .GetType()
            .GetProperty("message")?
            .GetValue(conflict.Value)?
            .ToString();


        Assert.AreEqual(
            "このカテゴリ名は既に登録されています。",
            message);
    }


    /// <summary>
    /// UT-REC-008
    /// カテゴリ名未入力の場合400を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenCategoryNameEmpty()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = ""
        };


        // Act
        var result = await _controller.Register(request);


        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));


        var badRequest = (BadRequestObjectResult)result;


        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);


        var message = badRequest.Value!
            .GetType()
            .GetProperty("message")?
            .GetValue(badRequest.Value)?
            .ToString();


        Assert.AreEqual(
           "入力値に不備があります。",
            message);


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }


    /// <summary>
    /// UT-REC-009
    /// カテゴリ名空白の場合400を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenCategoryNameWhitespace()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "   "
        };


        // Act
        var result = await _controller.Register(request);


        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }


    /// <summary>
    /// UT-REC-010
    /// 31文字の場合400を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenCategoryNameLengthIs31()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = new string('あ', 31)
        };


        _controller.ModelState.AddModelError(
            "CategoryName",
            "カテゴリ名は30文字以内で入力してください。");


        // Act
        var result = await _controller.Register(request);


        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }


    /// <summary>
    /// UT-REC-011
    /// 1文字でも登録できること
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsCreated_WhenCategoryNameLengthIs1()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食"
        };


        // Act
        var result = await _controller.Register(request);


        // Assert
        var objectResult = result as ObjectResult;


        Assert.IsNotNull(objectResult);

        Assert.AreEqual(
            StatusCodes.Status201Created,
            objectResult.StatusCode);


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Once);
    }


    /// <summary>
    /// UT-REC-012
    /// 30文字でも登録できること
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsCreated_WhenCategoryNameLengthIs30()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = new string('あ', 30)
        };


        // Act
        var result = await _controller.Register(request);


        // Assert
        var objectResult = result as ObjectResult;


        Assert.IsNotNull(objectResult);


        Assert.AreEqual(
            StatusCodes.Status201Created,
            objectResult.StatusCode);


        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Once);
    }
}