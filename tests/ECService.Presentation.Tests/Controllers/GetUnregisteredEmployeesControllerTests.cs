using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

[TestClass]
public class GetUnregisteredEmployeesControllerTests
{
    private Mock<IGetUnregisteredEmployeesUsecase> _usecaseMock = null!;
    private UnregisteredEmployeesViewModelAdapter _adapter = null!;
    private GetUnregisteredEmployeesController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _usecaseMock =
            new Mock<IGetUnregisteredEmployeesUsecase>();

        _adapter =
            new UnregisteredEmployeesViewModelAdapter();

        _controller =
            new GetUnregisteredEmployeesController(
                _usecaseMock.Object,
                _adapter);
    }

    [TestMethod(DisplayName = "未登録社員一覧を取得すると200と社員一覧を返す")]
    public async Task GetUnregisteredEmployees_ShouldReturnOk_WithEmployees()
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

        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync())
            .ReturnsAsync(employees);

        // Act
        var result =
            await _controller.GetUnregisteredEmployees();

        // Assert
        var okResult =
            result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response =
            okResult.Value as UnregisteredEmployeesResponse;

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Employees);
        Assert.HasCount(2, response.Employees);

        Assert.AreEqual(
            "22222222-2222-2222-2222-222222222222",
            response.Employees[0].EmployeeUuid);

        Assert.AreEqual(
            "山田太郎",
            response.Employees[0].EmployeeName);

        Assert.AreEqual(
            "33333333-3333-3333-3333-333333333333",
            response.Employees[1].EmployeeUuid);

        Assert.AreEqual(
            "佐藤花子",
            response.Employees[1].EmployeeName);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(),
            Times.Once);
    }

    [TestMethod(DisplayName = "未登録社員が0件の場合は200と空の一覧を返す")]
    public async Task GetUnregisteredEmployees_ShouldReturnOk_WithEmptyList()
    {
        // Arrange
        _usecaseMock
            .Setup(usecase => usecase.ExecuteAsync())
            .ReturnsAsync(new List<Employee>());

        // Act
        var result =
            await _controller.GetUnregisteredEmployees();

        // Assert
        var okResult =
            result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var response =
            okResult.Value as UnregisteredEmployeesResponse;

        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Employees);
        Assert.IsEmpty(response.Employees);

        _usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(),
            Times.Once);
    }
}