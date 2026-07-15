using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Domain.Exceptions;
using ECService.Application.Authentications;
using ECService.Application.Usecases.Interfaces;
using ECService.Application.Exceptions;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Application.Usecases.Imps;
using Moq;
namespace ECService.Application.Tests.Usecases;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するインターフェイス実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/EmployeeAccounts/Interactor")]
public class RegisterEmployeeAccountUsecaseTests
{
    private Mock<IEmployeeAccountRepository> _employeeAccountRepositoryMock = null!;
    private Mock<IPasswordService> _passwordServiceMock = null!;
    private Mock<IEmployeeRepository> _employeeRepositoryMock = null!;
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;

    private RegisterEmployeeAccountUsecase _usecase = null!;

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _employeeAccountRepositoryMock =
            new Mock<IEmployeeAccountRepository>();

        _passwordServiceMock =
            new Mock<IPasswordService>();

        _employeeRepositoryMock =
            new Mock<IEmployeeRepository>();

        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _usecase = new RegisterEmployeeAccountUsecase(
            _employeeAccountRepositoryMock.Object,
            _passwordServiceMock.Object,
            _employeeRepositoryMock.Object,
            _unitOfWorkMock.Object);

        // トランザクション処理の共通設定
        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.BeginTransactionAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CommitAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.RollbackAsync())
            .Returns(Task.CompletedTask);
    }


    [TestMethod(DisplayName = "有効な担当者アカウントを登録できる")]
    public async Task ExecuteAsync_ShouldRegister_WhenValidEmployeeAccount()
    {
        // Arrange
        var employeeUuid =
             Guid.NewGuid().ToString();

        const string accountName = "taro1";
        const string rawPassword = "Pssw0rd123";
        const string passwordHash = "hashed-password";

        var employee = CreateEmployeeForTest(employeeUuid);

        EmployeeAccount? createdAccount = null;

        // アカウント名は重複していない
        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.ExistsByAccountNameAsync(accountName))
            .ReturnsAsync(false);

        // 対象の社員は存在する
        _employeeRepositoryMock
            .Setup(repository =>
                repository.SelectByUuidAsync(employeeUuid))
            .ReturnsAsync(employee);

        // パスワードのハッシュ化結果
        _passwordServiceMock
            .Setup(service => service.Hash(rawPassword))
            .Returns(passwordHash);

        // CreateAsyncに渡されたアカウントを保存する
        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.CreateAsync(It.IsAny<EmployeeAccount>()))
            .Callback<EmployeeAccount>(account =>
                createdAccount = account)
            .Returns(Task.CompletedTask);

        // Act
        await _usecase.ExecuteAsync(
            employeeUuid,
            accountName,
            rawPassword);

        // Assert
        Assert.IsNotNull(
            createdAccount,
            "登録対象の担当者アカウントが生成されていません。");



    }

    [TestMethod(
        DisplayName =
            "重複するアカウント名の場合はExistsAccountExceptionをスローする")]
    public async Task ExecuteAsync_ShouldThrowExistsAccountException_WhenAccountNameAlreadyExists()
    {
        // Arrange
        var employeeUuid =
              Guid.NewGuid().ToString();
        const string accountName = "taro1";
        const string rawPassword = "Pssw0rd123";

        // アカウント名が既に存在する
        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.ExistsByAccountNameAsync(accountName))
            .ReturnsAsync(true);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsAccountException>(
                () => _usecase.ExecuteAsync(
                    employeeUuid,
                    accountName,
                    rawPassword));

        // Assert
        Assert.AreEqual(
            "このアカウント名は既に登録されています。",
            exception.Message);
    }

    [TestMethod(
        DisplayName =
            "社員が存在しない場合はExistsEmployeeExceptionをスローする")]
    public async Task ExecuteAsync_ShouldThrowExistsEmployeeException_WhenEmployeeDoesNotExist()
    {
        // Arrange
        const string employeeUuid =
            "11111111-1111-1111-1111-111111111111";

        const string accountName = "taro1";
        const string rawPassword = "Pssw0rd123";

        // アカウント名は重複していない
        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.ExistsByAccountNameAsync(accountName))
            .ReturnsAsync(false);

        // 社員が存在しない
        _employeeRepositoryMock
            .Setup(repository =>
                repository.SelectByUuidAsync(employeeUuid))
            .ReturnsAsync((Employee?)null);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsEmployeeException>(
                () => _usecase.ExecuteAsync(
                    employeeUuid,
                    accountName,
                    rawPassword));

        // Assert
        Assert.AreEqual(
            "指定された社員IDが存在しません。",
            exception.Message);
     
    }

    [TestMethod(
        DisplayName =
            "アカウント登録処理で例外が発生した場合はロールバックする")]
    public async Task ExecuteAsync_ShouldRollback_WhenRepositoryThrowsException()
    {
        // Arrange
        const string employeeUuid =
            "11111111-1111-1111-1111-111111111111";

        const string accountName = "taro1";
        const string rawPassword = "Pssw0rd123";
        const string passwordHash = "hashed-password";

        var employee = CreateEmployeeForTest(employeeUuid);

        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.ExistsByAccountNameAsync(accountName))
            .ReturnsAsync(false);

        _employeeRepositoryMock
            .Setup(repository =>
                repository.SelectByUuidAsync(employeeUuid))
            .ReturnsAsync(employee);

        _passwordServiceMock
            .Setup(service => service.Hash(rawPassword))
            .Returns(passwordHash);

        // 登録時に例外が発生するようにする
        _employeeAccountRepositoryMock
            .Setup(repository =>
                repository.CreateAsync(It.IsAny<EmployeeAccount>()))
            .ThrowsAsync(
                new InternalException(
                    "担当者アカウントの登録に失敗しました。"));

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _usecase.ExecuteAsync(
                    employeeUuid,
                    accountName,
                    rawPassword));

        // Assert
        Assert.AreEqual(
            "担当者アカウントの登録に失敗しました。",
            exception.Message);

        _employeeAccountRepositoryMock
    .Setup(repository =>
        repository.CreateAsync(It.IsAny<EmployeeAccount>()))
    .ThrowsAsync(new InternalException("登録失敗"));

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.RollbackAsync(),
            Times.Once);
    }

    /// <summary>
    /// テストで使用する社員を生成する
    /// </summary>
    private static Employee CreateEmployeeForTest(
        string employeeUuid)
    {
        var department = new Department(
            "22222222-2222-2222-2222-222222222222",
            "システム部");

        return new Employee(
            employeeUuid,
            "山田太郎",
            "ヤマダタロウ",
            department);
    }
}