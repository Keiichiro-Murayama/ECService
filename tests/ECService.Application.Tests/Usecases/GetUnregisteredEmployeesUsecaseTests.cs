using ECService.Application.Usecases.Imps;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

[TestClass]
public class GetUnregisteredEmployeesUsecaseTests
{
    private Mock<IEmployeeRepository> _employeeRepositoryMock = null!;
    private GetUnregisteredEmployeesUsecase _usecase = null!;

    [TestInitialize]
    public void Setup()
    {
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();

        _usecase = new GetUnregisteredEmployeesUsecase(
            _employeeRepositoryMock.Object);
    }

    [TestMethod(DisplayName = "未登録社員一覧を取得できる")]
    public async Task ExecuteAsync_ShouldReturnUnregisteredEmployees()
    {
        // Arrange
        var department = new Department(
            "11111111-1111-1111-1111-111111111111",
            "システム部");

        var employees = new List<Employee>
        {
            new Employee(
                "22222222-2222-2222-2222-222222222222",
                "山田太郎",
                "ヤマダタロウ",
                department),

            new Employee(
                "33333333-3333-3333-3333-333333333333",
                "佐藤花子",
                "サトウハナコ",
                department)
        };

        _employeeRepositoryMock
            .Setup(repository =>
                repository.SelectUnregisteredAsync())
            .ReturnsAsync(employees);

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.HasCount(2, result);

        Assert.AreEqual(
            "22222222-2222-2222-2222-222222222222",
            result[0].EmployeeUuid);

        Assert.AreEqual(
            "山田太郎",
            result[0].Name);

        Assert.AreEqual(
            "33333333-3333-3333-3333-333333333333",
            result[1].EmployeeUuid);

        Assert.AreEqual(
            "佐藤花子",
            result[1].Name);

        _employeeRepositoryMock.Verify(
            repository => repository.SelectUnregisteredAsync(),
            Times.Once);
    }

    [TestMethod(DisplayName = "未登録社員が存在しない場合は空の一覧を返す")]
    public async Task ExecuteAsync_ShouldReturnEmptyList_WhenNoEmployeesExist()
    {
        // Arrange
        _employeeRepositoryMock
            .Setup(repository =>
                repository.SelectUnregisteredAsync())
            .ReturnsAsync(new List<Employee>());

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.IsEmpty(result);

        _employeeRepositoryMock.Verify(
            repository => repository.SelectUnregisteredAsync(),
            Times.Once);
    }
}