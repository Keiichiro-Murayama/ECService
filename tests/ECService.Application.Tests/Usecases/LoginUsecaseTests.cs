using ECService.Application.Authentications;
using ECService.Application.Exceptions;
using ECService.Application.Usecases.Imps;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// LoginUsecase の単体テスト
/// </summary>
[TestClass]
public class LoginUsecaseTests
{
    private const string AccountUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    private const string EmployeeUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06c";

    private const string DepartmentUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06d";

    private const string AccountName = "admin01";

    private const string Password = "Pass123";

    /// <summary>
    /// 正しいユーザー名・パスワードが入力された場合、
    /// JWTが返却されることを確認する。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_LoginSuccess_ReturnsAccessToken()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeAccountRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var jwtProviderMock = new Mock<IJwtTokenProvider>();

        var department = new Department(
            DepartmentUuid,
            "営業部");

        var employee = new Employee(
            EmployeeUuid,
            "山田太郎",
            "ヤマダタロウ",
            department);

        var account = new EmployeeAccount(
            AccountUuid,
            AccountName,
            "hashedPassword",
            employee,
            null,
            0);

        repositoryMock
            .Setup(repository =>
                repository.SelectByAccountNameAsync(AccountName))
            .ReturnsAsync(account);

        passwordServiceMock
            .Setup(service =>
                service.Verify(account.PasswordHash, Password))
            .Returns(true);

        jwtProviderMock
            .Setup(provider =>
                provider.IssueAccessToken(account, null))
            .Returns("dummy-token");

        repositoryMock
            .Setup(repository =>
                repository.ResetLoginFailureAsync(AccountName))
            .Returns(Task.CompletedTask);

        var usecase = new LoginUsecase(
            repositoryMock.Object,
            passwordServiceMock.Object,
            jwtProviderMock.Object);

        // Act
        var result = await usecase.ExecuteAsync(
            (AccountName, Password));

        // Assert
        Assert.AreEqual("dummy-token", result.AccessToken);
        Assert.AreSame(account, result.EmployeeAccount);

        repositoryMock.Verify(
            repository =>
                repository.SelectByAccountNameAsync(AccountName),
            Times.Once);

        repositoryMock.Verify(
            repository =>
                repository.ResetLoginFailureAsync(AccountName),
            Times.Once);

        repositoryMock.Verify(
            repository =>
                repository.RecordLoginFailureAsync(AccountName),
            Times.Never);

        jwtProviderMock.Verify(
            provider =>
                provider.IssueAccessToken(account, null),
            Times.Once);
    }

    /// <summary>
    /// ユーザーが存在しない場合、
    /// AuthenticationException が発生することを確認する。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_UserNotFound_ThrowsAuthenticationException()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeAccountRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var jwtProviderMock = new Mock<IJwtTokenProvider>();

        repositoryMock
            .Setup(repository =>
                repository.SelectByAccountNameAsync(AccountName))
            .ReturnsAsync((EmployeeAccount?)null);

        var usecase = new LoginUsecase(
            repositoryMock.Object,
            passwordServiceMock.Object,
            jwtProviderMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<AuthenticationException>(
                () => usecase.ExecuteAsync(
                    (AccountName, Password)));

        // Assert
        Assert.AreEqual(
            "AuthenticationFailed",
            exception.ErrorCode);

        repositoryMock.Verify(
            repository =>
                repository.SelectByAccountNameAsync(AccountName),
            Times.Once);

        passwordServiceMock.Verify(
            service =>
                service.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        jwtProviderMock.Verify(
            provider =>
                provider.IssueAccessToken(
                    It.IsAny<EmployeeAccount>(),
                    It.IsAny<IEnumerable<System.Security.Claims.Claim>>()),
            Times.Never);
    }

    /// <summary>
    /// パスワードが一致しない場合、
    /// AuthenticationException が発生することを確認する。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_InvalidPassword_ThrowsAuthenticationException()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeAccountRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var jwtProviderMock = new Mock<IJwtTokenProvider>();

        var department = new Department(
            DepartmentUuid,
            "営業部");

        var employee = new Employee(
            EmployeeUuid,
            "山田太郎",
            "ヤマダタロウ",
            department);

        var account = new EmployeeAccount(
            AccountUuid,
            AccountName,
            "hashedPassword",
            employee,
            null,
            0);

        repositoryMock
            .Setup(repository =>
                repository.SelectByAccountNameAsync(AccountName))
            .ReturnsAsync(account);

        passwordServiceMock
            .Setup(service =>
                service.Verify(account.PasswordHash, Password))
            .Returns(false);

        repositoryMock
            .Setup(repository =>
                repository.RecordLoginFailureAsync(AccountName))
            .Returns(Task.CompletedTask);

        var usecase = new LoginUsecase(
            repositoryMock.Object,
            passwordServiceMock.Object,
            jwtProviderMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<AuthenticationException>(
                () => usecase.ExecuteAsync(
                    (AccountName, Password)));

        // Assert
        Assert.AreEqual(
            "AuthenticationFailed",
            exception.ErrorCode);

        repositoryMock.Verify(
            repository =>
                repository.RecordLoginFailureAsync(AccountName),
            Times.Once);

        repositoryMock.Verify(
            repository =>
                repository.ResetLoginFailureAsync(AccountName),
            Times.Never);

        jwtProviderMock.Verify(
            provider =>
                provider.IssueAccessToken(
                    It.IsAny<EmployeeAccount>(),
                    It.IsAny<IEnumerable<System.Security.Claims.Claim>>()),
            Times.Never);
    }

    /// <summary>
    /// アカウントがロック中の場合、
    /// AuthenticationException(AccountLocked)が発生することを確認する。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_AccountLocked_ThrowsAuthenticationException()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeAccountRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var jwtProviderMock = new Mock<IJwtTokenProvider>();

        var department = new Department(
            DepartmentUuid,
            "営業部");

        var employee = new Employee(
            EmployeeUuid,
            "山田太郎",
            "ヤマダタロウ",
            department);

        var account = new EmployeeAccount(
            AccountUuid,
            AccountName,
            "hashedPassword",
            employee,
            DateTime.UtcNow.AddMinutes(30),
            3);

        repositoryMock
            .Setup(repository =>
                repository.SelectByAccountNameAsync(AccountName))
            .ReturnsAsync(account);

        var usecase = new LoginUsecase(
            repositoryMock.Object,
            passwordServiceMock.Object,
            jwtProviderMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<AuthenticationException>(
                () => usecase.ExecuteAsync(
                    (AccountName, Password)));

        // Assert
        Assert.AreEqual(
            "AccountLocked",
            exception.ErrorCode);

        repositoryMock.Verify(
            repository =>
                repository.SelectByAccountNameAsync(AccountName),
            Times.Once);

        passwordServiceMock.Verify(
            service =>
                service.Verify(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);

        repositoryMock.Verify(
            repository =>
                repository.RecordLoginFailureAsync(It.IsAny<string>()),
            Times.Never);

        repositoryMock.Verify(
            repository =>
                repository.ResetLoginFailureAsync(It.IsAny<string>()),
            Times.Never);

        jwtProviderMock.Verify(
            provider =>
                provider.IssueAccessToken(
                    It.IsAny<EmployeeAccount>(),
                    It.IsAny<IEnumerable<System.Security.Claims.Claim>>()),
            Times.Never);
    }

    /// <summary>
    /// ロック解除済みのアカウントの場合、
    /// 正常にログインできることを確認する。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_UnlockedAccount_ReturnsAccessToken()
    {
        // Arrange
        var repositoryMock = new Mock<IEmployeeAccountRepository>();
        var passwordServiceMock = new Mock<IPasswordService>();
        var jwtProviderMock = new Mock<IJwtTokenProvider>();

        var department = new Department(
            DepartmentUuid,
            "営業部");

        var employee = new Employee(
            EmployeeUuid,
            "山田太郎",
            "ヤマダタロウ",
            department);

        var account = new EmployeeAccount(
            AccountUuid,
            AccountName,
            "hashedPassword",
            employee,
            DateTime.UtcNow.AddMinutes(-10),
            3);

        repositoryMock
            .Setup(repository =>
                repository.SelectByAccountNameAsync(AccountName))
            .ReturnsAsync(account);

        passwordServiceMock
            .Setup(service =>
                service.Verify(account.PasswordHash, Password))
            .Returns(true);

        jwtProviderMock
            .Setup(provider =>
                provider.IssueAccessToken(account, null))
            .Returns("dummy-token");

        repositoryMock
            .Setup(repository =>
                repository.ResetLoginFailureAsync(AccountName))
            .Returns(Task.CompletedTask);

        var usecase = new LoginUsecase(
            repositoryMock.Object,
            passwordServiceMock.Object,
            jwtProviderMock.Object);

        // Act
        var result =
            await usecase.ExecuteAsync((AccountName, Password));

        // Assert
        Assert.AreEqual(
            "dummy-token",
            result.AccessToken);

        repositoryMock.Verify(
            repository =>
                repository.ResetLoginFailureAsync(AccountName),
            Times.Once);

        jwtProviderMock.Verify(
            provider =>
                provider.IssueAccessToken(account, null),
            Times.Once);
    }

}