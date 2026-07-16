using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// EmployeeEntityAdapterの単体テスト
/// </summary>
[TestClass]
public class EmployeeEntityAdapterTests
{
    [TestMethod(DisplayName = "UT-ADP-001 EmployeeEntityをEmployeeドメインへ復元できる")]
    public async Task RestoreAsync_ShouldRestoreEmployeeEntityToDomain()
    {
        // Arrange
        var adapter = new EmployeeEntityAdapter();

        var departmentEntity = new DepartmentEntity
        {
            Id = 1,
            DepartmentUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "システム部"
        };

        var employeeEntity = new EmployeeEntity
        {
            Id = 1,
            EmployeeUuid = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "山田太郎",
            Kana = "ヤマダタロウ",
            DepartmentId = 1,
            Department = departmentEntity
        };

        // Act
        var employee = await adapter.RestoreAsync(employeeEntity);

        // Assert
        Assert.AreEqual(
            "22222222-2222-2222-2222-222222222222",
            employee.EmployeeUuid);

        Assert.AreEqual("山田太郎", employee.Name);
        Assert.AreEqual("ヤマダタロウ", employee.Kana);
    }

    [TestMethod(DisplayName = "UT-ADP-002 部署情報を含めてEmployeeドメインへ復元できる")]
    public async Task RestoreAsync_ShouldRestoreDepartment()
    {
        // Arrange
        var adapter = new EmployeeEntityAdapter();

        var departmentEntity = new DepartmentEntity
        {
            Id = 1,
            DepartmentUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "営業部"
        };

        var employeeEntity = new EmployeeEntity
        {
            Id = 1,
            EmployeeUuid = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Name = "佐藤美咲",
            Kana = "サトウミサキ",
            DepartmentId = 1,
            Department = departmentEntity
        };

        // Act
        var employee = await adapter.RestoreAsync(employeeEntity);

        // Assert
        Assert.AreEqual(
            "11111111-1111-1111-1111-111111111111",
            employee.Department.DepartmentUuid);

        Assert.AreEqual("営業部", employee.Department.Name);
    }
}