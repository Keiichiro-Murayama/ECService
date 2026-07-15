using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;
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
    /// カテゴリを正常に登録できること
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsCreated_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));

        var objectResult = (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status201Created,
            objectResult.StatusCode);

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Once);
    }

    /// <summary>
    /// UT-REC-007
    /// カテゴリ名が重複していること
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsConflict_WhenCategoryAlreadyExists()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(
                new DomainException("カテゴリ名:食品は既に存在します。"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));

        var conflictResult = (ConflictObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflictResult.StatusCode);
    }

    /// <summary>
    /// UT-REC-008
    /// カテゴリ名が未入力
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenCategoryNameIsEmpty()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = string.Empty
        };

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }

    /// <summary>
    /// UT-REC-009
    /// カテゴリ名が空白のみ
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenCategoryNameIsWhitespace()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "     "
        };

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }

    /// <summary>
    /// UT-REC-010
    /// ModelStateが不正
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _controller.ModelState.AddModelError(
            "CategoryName",
            "入力エラー");

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }
        /// <summary>
    /// UT-REC-011
    /// DomainException（重複以外）は400を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsBadRequest_WhenDomainExceptionOccurs()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(new DomainException("カテゴリ名が不正です"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        var badRequest = (BadRequestObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequest.StatusCode);
    }

    /// <summary>
    /// UT-REC-012
    /// InternalExceptionは500を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsInternalServerError_WhenInternalExceptionOccurs()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(new InternalException("DB Error"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));

        var objectResult = (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);
    }

    /// <summary>
    /// UT-REC-013
    /// Exception（重複メッセージ）は409を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsConflict_WhenExceptionContainsAlreadyExists()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(new Exception("既に登録されています"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ConflictObjectResult));

        var conflict = (ConflictObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflict.StatusCode);
    }

    /// <summary>
    /// UT-REC-014
    /// Exception（通常）は500を返すこと
    /// </summary>
    [TestMethod]
    public async Task Register_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "食品"
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<ProductCategory>()))
            .ThrowsAsync(new Exception("Unexpected Error"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsInstanceOfType(result, typeof(ObjectResult));

        var objectResult = (ObjectResult)result;

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            objectResult.StatusCode);
    }

    /// <summary>
    /// UT-REC-015
    /// CategoryNameが31文字でModelStateエラーの場合は400を返すこと
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
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));

        _usecaseMock.Verify(
            x => x.ExecuteAsync(It.IsAny<ProductCategory>()),
            Times.Never);
    }
}