using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Controllers;
using ECService.Domain.Exceptions;
using ECService.Presentation.ViewModels;
using ECService.Application.Exceptions;
using Moq;

namespace ECService.Presentation.Tests.Controllers;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するコントローラのテストドライバ
/// </summary>
[TestClass]
[TestCategory("Controllers")]
public class RegisterEmployeeAccountControllerTests
{
    private Mock<IRegisterEmployeeAccountUsecase> _usecaseMock = null!;
    private RegisterEmployeeAccountController _controller = null!;

    /// <summary>
    /// テストメソッド実行の前処理
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _usecaseMock = new Mock<IRegisterEmployeeAccountUsecase>();

        _controller = new RegisterEmployeeAccountController(
            _usecaseMock.Object);
    }


    [TestMethod(DisplayName = "アカウント名が重複している場合は409を返す")]
    public async Task Register_ShouldReturnBadRequest_WhenAccountAlreadyExists()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password))
            .ThrowsAsync(
                new ExistsAccountException(
                    "このアカウント名は既に使用されています。"));
        // Act
        var result = await _controller.Register(request);

        // Assert
        var conflictResult = result as ConflictObjectResult;

        Assert.IsNotNull(conflictResult);
        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            conflictResult.StatusCode);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password),
            Times.Once);
    }

    [TestMethod(DisplayName = "社員が存在しない場合は404を返す")]
    public async Task Register_ShouldReturn404_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password))
            .ThrowsAsync(
                new ExistsEmployeeException(
                    "選択された社員が存在しません。"));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);
        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            notFoundResult.StatusCode);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password),
            Times.Once);
    }
    [TestMethod(DisplayName = "ドメインルール違反の場合は400を返す")]
    public async Task Register_ShouldReturn400_WhenModelInvalid()
    {
        // Arrange
        var request = CreateValidRequest();

        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password))
            .ThrowsAsync(
                new DomainException(
                    "アカウント名は5～20文字で入力してください",
                    nameof(request.AccountName)));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;

        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password),
            Times.Once);
    }

    [TestMethod(DisplayName = "有効なリクエストの場合は201を返す")]
    public async Task Register_ShouldReturn201_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterEmployeeAccountRequest
        {
            EmployeeUuid = "11111111-1111-1111-1111-111111111111",
            AccountName = "taro1",
            Password = "Password123"
        };

        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var objectResult = result as ObjectResult;

        Assert.IsNotNull(objectResult);
        Assert.AreEqual(
            StatusCodes.Status201Created,
            objectResult.StatusCode);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password),
            Times.Once);
    }
    private static RegisterEmployeeAccountRequest CreateValidRequest()
    {
        return new RegisterEmployeeAccountRequest
        {
            EmployeeUuid = "11111111-1111-1111-1111-111111111111",
            AccountName = "taro1",
            Password = "Password123"
        };
    }
}