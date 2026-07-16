using ECService.Application.Exceptions;
using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using ECService.Presentations.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// AuthControllerの単体テスト
/// </summary>
[TestClass]
public class AuthControllerTests
{
    /// <summary>
    /// テスト用アカウント情報
    /// </summary>
    private const string AccountUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    private const string EmployeeUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06c";

    private const string DepartmentUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06d";

    private const string Username = "testuser";
    private const string Password = "password";
    private const string AccessToken = "dummy-token";

    /// <summary>
    /// ログイン成功時、
    /// 200 OKが返却されることを確認する。
    /// </summary>
    [TestMethod]
    public async Task Login_LoginSucceeds_ReturnsOk()
    {
        // Arrange
        var usecaseMock =
            new Mock<ILoginUsecase>();

        var department =
            new Department(
                DepartmentUuid,
                "営業部");

        var employee =
            new Employee(
                EmployeeUuid,
                "山田太郎",
                "ヤマダタロウ",
                department);

        var account =
            new EmployeeAccount(
                AccountUuid,
                Username,
                "hashedPassword",
                employee);

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)))
            .ReturnsAsync((AccessToken, account));
        var controller =
            new AuthController(
                usecaseMock.Object,
                new LoginViewModelAdapter());

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

        var request =
            new LoginRequest
            {
                Username = Username,
                Password = Password
            };

        // Act
        var result =
            await controller.Login(request);

        // Assert
        Assert.IsInstanceOfType(
            result.Result,
            typeof(OkObjectResult));

        var okResult =
            result.Result as OkObjectResult;

        Assert.IsNotNull(okResult);

        Assert.IsInstanceOfType(
            okResult.Value,
            typeof(TokenResponse));

        var response =
            okResult.Value as TokenResponse;

        Assert.AreEqual(
            "ログインに成功しました。",
            response!.Message);

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)), Times.Once);
    }

    /// <summary>
    /// 認証失敗時、
    /// 401 Unauthorizedが返却されることを確認する。
    /// </summary>
    [TestMethod]
    public async Task Login_AuthenticationFailed_ReturnsUnauthorized()
    {
        // Arrange
        var usecaseMock =
            new Mock<ILoginUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)))
            .ThrowsAsync(new AuthenticationException(
                "AuthenticationFailed",
                "ユーザー名またはパスワードが正しくありません。"));

        var controller =
            new AuthController(
                usecaseMock.Object,
                new LoginViewModelAdapter());

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

        var request =
            new LoginRequest
            {
                Username = Username,
                Password = Password
            };

        // Act
        var result =
            await controller.Login(request);

        // Assert
        Assert.IsInstanceOfType(
            result.Result,
            typeof(UnauthorizedObjectResult));

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)), Times.Once);
    }

    /// <summary>
    /// アカウントロック中の場合、
    /// 423 Lockedが返却されることを確認する。
    /// </summary>
    [TestMethod]
    public async Task Login_AccountLocked_ReturnsLocked()
    {
        // Arrange
        var usecaseMock =
            new Mock<ILoginUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)))
            .ThrowsAsync(new AuthenticationException(
                "AccountLocked",
                "アカウントはロックされています。",
                30));

        var controller =
            new AuthController(
                usecaseMock.Object,
                new LoginViewModelAdapter());

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

        var request =
            new LoginRequest
            {
                Username = Username,
                Password = Password
            };

        // Act
        var result =
            await controller.Login(request);

        // Assert
        Assert.IsInstanceOfType(
            result.Result,
            typeof(ObjectResult));

        var objectResult =
            result.Result as ObjectResult;

        Assert.IsNotNull(objectResult);

        Assert.AreEqual(
            StatusCodes.Status423Locked,
            objectResult.StatusCode);

        usecaseMock.Verify(usecase => usecase.ExecuteAsync(It.Is<(string, string)>(p => p.Item1 == Username && p.Item2 == Password)), Times.Once);
    }
    /// <summary>
/// ユーザー名未入力の場合、
/// 400 BadRequestが返却されることを確認する。
/// </summary>
[TestMethod]
public async Task Login_UsernameIsEmpty_ReturnsBadRequest()
{
    // Arrange
    var usecaseMock = new Mock<ILoginUsecase>();

    var controller = new AuthController(
        usecaseMock.Object,
        new LoginViewModelAdapter());

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };

    controller.ModelState.AddModelError(
        nameof(LoginRequest.Username),
        "ユーザー名は必須項目です");

    var request = new LoginRequest
    {
        Username = "",
        Password = Password
    };

    // Act
    var result = await controller.Login(request);

    // Assert
    Assert.IsInstanceOfType(
        result.Result,
        typeof(ObjectResult));

    var badRequest =
        result.Result as ObjectResult;

    Assert.IsNotNull(badRequest);

    Assert.AreEqual(
        StatusCodes.Status400BadRequest,
        badRequest.StatusCode);

    usecaseMock.Verify(
        usecase => usecase.ExecuteAsync(It.IsAny<(string, string)>()),
        Times.Never);
}
[TestMethod]
public async Task Login_PasswordIsEmpty_ReturnsBadRequest()
{
    // Arrange
    var usecaseMock = new Mock<ILoginUsecase>();

    var controller = new AuthController(
        usecaseMock.Object,
        new LoginViewModelAdapter());

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };

    controller.ModelState.AddModelError(
        nameof(LoginRequest.Password),
        "パスワードは必須項目です");

    var request = new LoginRequest
    {
        Username = Username,
        Password = ""
    };

    // Act
    var result = await controller.Login(request);

    // Assert
    Assert.IsInstanceOfType(
        result.Result,
        typeof(ObjectResult));

    var badRequest =
        result.Result as ObjectResult;

    Assert.IsNotNull(badRequest);

    Assert.AreEqual(
        StatusCodes.Status400BadRequest,
        badRequest.StatusCode);

    usecaseMock.Verify(
        usecase => usecase.ExecuteAsync(It.IsAny<(string, string)>()),
        Times.Never);
}
[TestMethod]
public async Task Login_UsernameAndPasswordAreEmpty_ReturnsBadRequest()
{
    // Arrange
    var usecaseMock = new Mock<ILoginUsecase>();

    var controller = new AuthController(
        usecaseMock.Object,
        new LoginViewModelAdapter());

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };

    controller.ModelState.AddModelError(
        nameof(LoginRequest.Username),
        "ユーザー名は必須項目です");

    controller.ModelState.AddModelError(
        nameof(LoginRequest.Password),
        "パスワードは必須項目です");

    var request = new LoginRequest
    {
        Username = "",
        Password = ""
    };

    // Act
    var result = await controller.Login(request);

    // Assert
    Assert.IsInstanceOfType(
        result.Result,
        typeof(ObjectResult));

    var badRequest =
        result.Result as ObjectResult;

    Assert.IsNotNull(badRequest);

    Assert.AreEqual(
        StatusCodes.Status400BadRequest,
        badRequest.StatusCode);

    usecaseMock.Verify(
        usecase => usecase.ExecuteAsync(It.IsAny<(string, string)>()),
        Times.Never);
}
/// <summary>
/// ログアウト時、
/// 200 OKが返却されることを確認する。
/// </summary>
[TestMethod]
public void Logout_ReturnsOk()
{
    // Arrange
    var usecaseMock = new Mock<ILoginUsecase>();

    var controller = new AuthController(
        usecaseMock.Object,
        new LoginViewModelAdapter());

    controller.ControllerContext = new ControllerContext
    {
        HttpContext = new DefaultHttpContext()
    };

    // Act
    var result = controller.Logout();

    // Assert
    Assert.IsInstanceOfType(
        result,
        typeof(OkObjectResult));

    var okResult = result as OkObjectResult;

    Assert.IsNotNull(okResult);
}
}