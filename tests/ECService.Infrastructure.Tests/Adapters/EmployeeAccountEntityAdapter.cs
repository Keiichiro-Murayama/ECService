using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// EmployeeAccountEntityAdapterの単体テスト
/// </summary>
[TestClass]
public class EmployeeAccountEntityAdapterTests
{
    [TestMethod(DisplayName = "UT-ADP-003 EmployeeAccountドメインをEmployeeAccountEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertEmployeeAccountDomainToEntity()
    {
        // Arrange
        var adapter = new EmployeeAccountEntityAdapter();

        var employeeAccount = CreateEmployeeAccount();

        // Act
        var entity = await adapter.ConvertAsync(employeeAccount);

        // Assert
        Assert.AreEqual(
            Guid.Parse("44444444-4444-4444-4444-444444444444"),
            entity.AccountUuid);

        Assert.AreEqual("user01", entity.Name);
        Assert.AreEqual("hash-value", entity.Password);
    }

    [TestMethod(DisplayName = "UT-ADP-004 EmployeeAccountEntityをEmployeeAccountドメインへ復元できる")]
    public async Task RestoreAsync_ShouldRestoreEmployeeAccountEntityToDomain()
    {
        // Arrange
        var adapter = new EmployeeAccountEntityAdapter();

        var employeeAccountEntity = CreateEmployeeAccountEntity();

        // Act
        var employeeAccount = await adapter.RestoreAsync(employeeAccountEntity);

        // Assert
        Assert.AreEqual(
            "44444444-4444-4444-4444-444444444444",
            employeeAccount.AccountUuid);

        Assert.AreEqual("user01", employeeAccount.AccountName);
        Assert.AreEqual("hash-value", employeeAccount.PasswordHash);
    }

    [TestMethod(DisplayName = "UT-ADP-005 ロックアウト情報をEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertLockoutValues()
    {
        // Arrange
        var adapter = new EmployeeAccountEntityAdapter();

        var employee = CreateEmployee();

        var lockoutEnd = new DateTime(
            2026,
            7,
            16,
            10,
            0,
            0,
            DateTimeKind.Utc);

        var employeeAccount = new EmployeeAccount(
            "44444444-4444-4444-4444-444444444444",
            "user01",
            "pass01",
            "hash-value",
            employee,
            lockoutEnd,
            3);

        // Act
        var entity = await adapter.ConvertAsync(employeeAccount);

        // Assert
        Assert.AreEqual(lockoutEnd, entity.LockoutEnd);
        Assert.AreEqual(3, entity.AccessFailedCount);
    }

    [TestMethod(DisplayName = "UT-ADP-006 紐づく社員情報を含めて復元できる")]
    public async Task RestoreAsync_ShouldRestoreEmployee()
    {
        // Arrange
        var adapter = new EmployeeAccountEntityAdapter();

        var employeeAccountEntity = CreateEmployeeAccountEntity();

        // Act
        var employeeAccount = await adapter.RestoreAsync(employeeAccountEntity);

        // Assert
        Assert.AreEqual(
            "22222222-2222-2222-2222-222222222222",
            employeeAccount.Employee.EmployeeUuid);

        Assert.AreEqual("山田太郎", employeeAccount.Employee.Name);
        Assert.AreEqual("ヤマダタロウ", employeeAccount.Employee.Kana);
        Assert.AreEqual("システム部", employeeAccount.Employee.Department.Name);
    }

    private static Department CreateDepartment()
    {
        return new Department(
            "11111111-1111-1111-1111-111111111111",
            "システム部");
    }

    private static Employee CreateEmployee()
    {
        return new Employee(
            "22222222-2222-2222-2222-222222222222",
            "山田太郎",
            "ヤマダタロウ",
            CreateDepartment());
    }

    private static EmployeeAccount CreateEmployeeAccount()
    {
        return new EmployeeAccount(
            "44444444-4444-4444-4444-444444444444",
            "user01",
            "pass01",
            "hash-value",
            CreateEmployee(),
            null,
            0);
    }

    private static EmployeeAccountEntity CreateEmployeeAccountEntity()
    {
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

        return new EmployeeAccountEntity
        {
            Id = 1,
            AccountUuid = Guid.Parse("44444444-4444-4444-4444-444444444444"),
            EmployeeId = 1,
            Name = "user01",
            Password = "hash-value",
            Employee = employeeEntity,
            LockoutEnd = null,
            AccessFailedCount = 0
        };
    }
}