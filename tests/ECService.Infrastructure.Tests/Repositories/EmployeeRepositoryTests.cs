using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

/// <summary>
/// EmployeeRepositoryの単体テスト
/// </summary>
[TestClass]
public class EmployeeRepositoryTests
{
    private AppDbContext _context = null!;
    private EmployeeRepository _repository = null!;

    private readonly List<Guid> _departmentUuids = new();
    private readonly List<Guid> _employeeUuids = new();
    private readonly List<Guid> _accountUuids = new();

    [TestInitialize]
    public async Task Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettingsTests.json",
                optional: false,
                reloadOnChange: false)
                 .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration
            .GetConnectionString("ECServiceDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "appsettingsTests.json に ConnectionStrings:ECServiceDB が設定されていません。");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new AppDbContext(options);

        if (!await _context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用PostgreSQLへ接続できません。");
        }

        var adapter = new EmployeeEntityAdapter();

        _repository = new EmployeeRepository(
            _context,
            adapter);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_context is null)
        {
            return;
        }

        _context.ChangeTracker.Clear();

        if (_accountUuids.Count > 0)
        {
            var accounts = await _context.EmployeeAccounts
                .Where(account =>
                    _accountUuids.Contains(account.AccountUuid))
                .ToListAsync();

            _context.EmployeeAccounts.RemoveRange(accounts);

            await _context.SaveChangesAsync();
        }

        if (_employeeUuids.Count > 0)
        {
            var employees = await _context.Employees
                .Where(employee =>
                    _employeeUuids.Contains(employee.EmployeeUuid))
                .ToListAsync();

            _context.Employees.RemoveRange(employees);

            await _context.SaveChangesAsync();
        }

        if (_departmentUuids.Count > 0)
        {
            var departments = await _context.Departments
                .Where(department =>
                    _departmentUuids.Contains(department.DepartmentUuid))
                .ToListAsync();

            _context.Departments.RemoveRange(departments);

            await _context.SaveChangesAsync();
        }

        await _context.DisposeAsync();
    }

    [TestMethod(DisplayName = "UT-REP-EMP-001 担当者アカウント未登録の社員一覧を取得できる")]
    public async Task SelectUnregisteredAsync_ShouldReturnEmployeesWithoutAccount()
    {
        // Arrange
        var department = await InsertDepartmentAsync("未登録社員部署");

        var employee = await InsertEmployeeAsync(
            department,
            "未登録乃小川",
            "ミトウロクノオガワ");

        // Act
        var result = await _repository.SelectUnregisteredAsync();

        // Assert
        var actualEmployee = result.SingleOrDefault(actual =>
            actual.EmployeeUuid == employee.EmployeeUuid.ToString());

        Assert.IsNotNull(actualEmployee);
        Assert.AreEqual(employee.Name, actualEmployee.Name);
        Assert.AreEqual(employee.Kana, actualEmployee.Kana);
        Assert.AreEqual(department.Name, actualEmployee.Department.Name);
    }

    [TestMethod(DisplayName = "UT-REP-EMP-002 担当者アカウント登録済みの社員は未登録一覧に含まれない")]
    public async Task SelectUnregisteredAsync_ShouldNotReturnEmployeesWithAccount()
    {
        // Arrange
        var department = await InsertDepartmentAsync("登録済社員部署");

        var employee = await InsertEmployeeAsync(
            department,
            "登録済乃小川",
            "トウロクズミノオガワ");

        await InsertEmployeeAccountAsync(employee);

        // Act
        var result = await _repository.SelectUnregisteredAsync();

        // Assert
        var actualEmployee = result.SingleOrDefault(actual =>
            actual.EmployeeUuid == employee.EmployeeUuid.ToString());

        Assert.IsNull(actualEmployee);
    }

    [TestMethod(DisplayName = "UT-REP-EMP-003 社員UUIDで社員を取得できる")]
    public async Task SelectByUuidAsync_ShouldReturnEmployee_WhenEmployeeUuidExists()
    {
        // Arrange
        var department = await InsertDepartmentAsync("UUID検索部署");

        var employee = await InsertEmployeeAsync(
            department,
            "小倉優大",
            "オグラユウダイ");

        // Act
        var result = await _repository.SelectByUuidAsync(
            employee.EmployeeUuid.ToString());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(employee.EmployeeUuid.ToString(), result.EmployeeUuid);
        Assert.AreEqual(employee.Name, result.Name);
        Assert.AreEqual(employee.Kana, result.Kana);
        Assert.AreEqual(department.Name, result.Department.Name);
    }

    [TestMethod(DisplayName = "UT-REP-EMP-004 存在しない社員UUIDの場合はnullを返す")]
    public async Task SelectByUuidAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var employeeUuid = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.SelectByUuidAsync(employeeUuid);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod(DisplayName = "UT-REP-EMP-005 社員UUID形式が不正な場合はnullを返す")]
    public async Task SelectByUuidAsync_ShouldReturnNull_WhenEmployeeUuidFormatIsInvalid()
    {
        // Arrange
        var employeeUuid = "invalid-employee-uuid";

        // Act
        var result = await _repository.SelectByUuidAsync(employeeUuid);

        // Assert
        Assert.IsNull(result);
    }

    private async Task<DepartmentEntity> InsertDepartmentAsync(
        string name)
    {
        var department = new DepartmentEntity
        {
            DepartmentUuid = Guid.NewGuid(),
            Name = CreateUniqueName(name)
        };

        _departmentUuids.Add(department.DepartmentUuid);

        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();

        return department;
    }

    private async Task<EmployeeEntity> InsertEmployeeAsync(
        DepartmentEntity department,
        string name,
        string kana)
    {
        var employee = new EmployeeEntity
        {
            EmployeeUuid = Guid.NewGuid(),
            DepartmentId = department.Id,
            Department = department,
            Name = CreateUniqueName(name),
            Kana = CreateUniqueName(kana)
        };

        _employeeUuids.Add(employee.EmployeeUuid);

        await _context.Employees.AddAsync(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    private async Task<EmployeeAccountEntity> InsertEmployeeAccountAsync(
        EmployeeEntity employee)
    {
        var account = new EmployeeAccountEntity
        {
            AccountUuid = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Name = CreateUniqueAccountName(),
            Password = "hash-value",
            LockoutEnd = null,
            AccessFailedCount = 0
        };

        _accountUuids.Add(account.AccountUuid);

        await _context.EmployeeAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return account;
    }

    private static string CreateUniqueName(
        string prefix)
    {
        return $"{prefix}_{Guid.NewGuid():N}"[..30];
    }

    private static string CreateUniqueAccountName()
    {
        return $"acct{Guid.NewGuid():N}"[..20];
    }
}