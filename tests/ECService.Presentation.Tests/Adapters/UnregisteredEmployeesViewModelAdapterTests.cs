using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Presentation.Tests.Adapters;

[TestClass]
public class UnregisteredEmployeesViewModelAdapterTests
{
    private UnregisteredEmployeesViewModelAdapter _adapter = null!;

    [TestInitialize]
    public void Setup()
    {
        _adapter = new UnregisteredEmployeesViewModelAdapter();
    }

    [TestMethod(DisplayName = "未登録社員一覧をViewModelへ変換できる")]
    public async Task Convert_ShouldReturnResponse_WhenEmployeesExist()
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

        // Act
        var result = await _adapter.Convert(employees);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Employees);
        Assert.AreEqual(2, result.Employees.Count);

        Assert.AreEqual(
            "22222222-2222-2222-2222-222222222222",
            result.Employees[0].EmployeeUuid);

        Assert.AreEqual(
            "山田太郎",
            result.Employees[0].EmployeeName);

        Assert.AreEqual(
            "33333333-3333-3333-3333-333333333333",
            result.Employees[1].EmployeeUuid);

        Assert.AreEqual(
            "佐藤花子",
            result.Employees[1].EmployeeName);
    }

    [TestMethod(DisplayName = "未登録社員が0件の場合は空の一覧を返す")]
    public async Task Convert_ShouldReturnEmptyResponse_WhenEmployeesAreEmpty()
    {
        // Arrange
        var employees = new List<Employee>();

        // Act
        var result = await _adapter.Convert(employees);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Employees);
        Assert.AreEqual(0, result.Employees.Count);
    }
}