using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

/// <summary>
/// EmployeeAccountRepositoryの単体テスト
/// </summary>
[TestClass]
public class EmployeeAccountRepositoryTests
{
    private AppDbContext _context = null!;
    private EmployeeAccountRepository _repository = null!;

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

        var adapter = new EmployeeAccountEntityAdapter();

        _repository = new EmployeeAccountRepository(
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

    [TestMethod(DisplayName = "UT-REP-EAC-001 アカウント名で担当者アカウントを取得できる")]
    public async Task SelectByAccountNameAsync_ShouldReturnEmployeeAccount_WhenAccountNameExists()
    {
        // Arrange
        var department = await InsertDepartmentAsync("取得部署");

        var employee = await InsertEmployeeAsync(
            department,
            "取得乃村山",
            "シュトクノムラヤマ");

        var account = await InsertEmployeeAccountAsync(employee);

        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.SelectByAccountNameAsync(account.Name);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(account.AccountUuid.ToString(), result.AccountUuid);
        Assert.AreEqual(account.Name, result.AccountName);
        Assert.AreEqual(account.Password, result.PasswordHash);
        Assert.AreEqual(employee.EmployeeUuid.ToString(), result.Employee.EmployeeUuid);
        Assert.AreEqual(employee.Name, result.Employee.Name);
        Assert.AreEqual(department.Name, result.Employee.Department.Name);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-002 存在しないアカウント名の場合はnullを返す")]
    public async Task SelectByAccountNameAsync_ShouldReturnNull_WhenAccountNameDoesNotExist()
    {
        // Arrange
        var accountName = CreateUniqueAccountName();

        // Act
        var result = await _repository.SelectByAccountNameAsync(accountName);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-003 アカウント名が存在する場合はtrueを返す")]
    public async Task ExistsByAccountNameAsync_ShouldReturnTrue_WhenAccountNameExists()
    {
        // Arrange
        var department = await InsertDepartmentAsync("存在確認部署");

        var employee = await InsertEmployeeAsync(
            department,
            "存在確認乃村山",
            "ソンザイカクニンノムラヤマ");

        var account = await InsertEmployeeAccountAsync(employee);

        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.ExistsByAccountNameAsync(account.Name);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-004 アカウント名が存在しない場合はfalseを返す")]
    public async Task ExistsByAccountNameAsync_ShouldReturnFalse_WhenAccountNameDoesNotExist()
    {
        // Arrange
        var accountName = CreateUniqueAccountName();

        // Act
        var result = await _repository.ExistsByAccountNameAsync(accountName);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-005 社員UUIDに紐づくアカウントが存在する場合はtrueを返す")]
    public async Task ExistsByEmployeeUuidAsync_ShouldReturnTrue_WhenEmployeeAccountExists()
    {
        // Arrange
        var department = await InsertDepartmentAsync("社員UUID確認部署");

        var employee = await InsertEmployeeAsync(
            department,
            "社員UUID乃村山",
            "シャインユウユウアイディノムラヤマ");

        await InsertEmployeeAccountAsync(employee);

        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.ExistsByEmployeeUuidAsync(
            employee.EmployeeUuid.ToString());

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-006 社員UUIDに紐づくアカウントが存在しない場合はfalseを返す")]
    public async Task ExistsByEmployeeUuidAsync_ShouldReturnFalse_WhenEmployeeAccountDoesNotExist()
    {
        // Arrange
        var department = await InsertDepartmentAsync("社員UUID未登録部署");

        var employee = await InsertEmployeeAsync(
            department,
            "未登録乃村山",
            "ミトウロクノムラヤマ");

        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.ExistsByEmployeeUuidAsync(
            employee.EmployeeUuid.ToString());

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-007 社員UUID形式が不正な場合はfalseを返す")]
    public async Task ExistsByEmployeeUuidAsync_ShouldReturnFalse_WhenEmployeeUuidFormatIsInvalid()
    {
        // Arrange
        var employeeUuid = "invalid-employee-uuid";

        // Act
        var result = await _repository.ExistsByEmployeeUuidAsync(employeeUuid);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-008 担当者アカウントを登録できる")]
    public async Task CreateAsync_ShouldCreateEmployeeAccount()
    {
        // Arrange
        var department = await InsertDepartmentAsync("登録部署");

        var employee = await InsertEmployeeAsync(
            department,
            "登録乃村山",
            "トウロクノムラヤマ");

        var accountName = CreateUniqueAccountName();

        var employeeAccount = CreateEmployeeAccountDomain(
            employee,
            accountName);

        _accountUuids.Add(Guid.Parse(employeeAccount.AccountUuid));

        _context.ChangeTracker.Clear();

        // Act
        await _repository.CreateAsync(employeeAccount);

        // Assert
        var savedAccount = await _context.EmployeeAccounts
            .AsNoTracking()
            .Include(account => account.Employee)
            .SingleOrDefaultAsync(account =>
                account.AccountUuid == Guid.Parse(employeeAccount.AccountUuid));

        Assert.IsNotNull(savedAccount);
        Assert.AreEqual(accountName, savedAccount.Name);
        Assert.AreEqual(employeeAccount.PasswordHash, savedAccount.Password);
        Assert.AreEqual(employee.Id, savedAccount.EmployeeId);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-009 存在しない社員を指定して登録した場合は例外を送出する")]
    public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenEmployeeDoesNotExist()
    {
        // Arrange
        var department = new Department(
            Guid.NewGuid().ToString(),
            "存在しない部署");

        var employee = new Employee(
            Guid.NewGuid().ToString(),
            "存在しない社員",
            "ソンザイシナイシャイン",
            department);

        var employeeAccount = new EmployeeAccount(
            Guid.NewGuid().ToString(),
            CreateUniqueAccountName(),
            "pass123",
            "hashvalue123",
            employee,
            null,
            0);

        // Act & Assert
        try
        {
            await _repository.CreateAsync(employeeAccount);

            Assert.Fail("InvalidOperationExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InvalidOperationException exception)
        {
            Assert.AreEqual("社員が見つかりません。", exception.Message);
        }
    }

    [TestMethod(DisplayName = "UT-REP-EAC-010 ログイン失敗回数を増やせる")]
    public async Task RecordLoginFailureAsync_ShouldIncrementAccessFailedCount()
    {
        // Arrange
        var department = await InsertDepartmentAsync("失敗回数部署");

        var employee = await InsertEmployeeAsync(
            department,
            "失敗回数乃片野",
            "シッパイカイスウノカタノ");

        var account = await InsertEmployeeAccountAsync(
            employee,
            accessFailedCount: 1);

        _context.ChangeTracker.Clear();

        // Act
        await _repository.RecordLoginFailureAsync(account.Name);

        // Assert
        var savedAccount = await _context.EmployeeAccounts
            .AsNoTracking()
            .SingleAsync(saved =>
                saved.AccountUuid == account.AccountUuid);

        Assert.AreEqual(2, savedAccount.AccessFailedCount);
        Assert.IsNull(savedAccount.LockoutEnd);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-011 ログイン失敗5回でロックアウト時刻が設定される")]
    public async Task RecordLoginFailureAsync_ShouldSetLockoutEnd_WhenFailureCountReachesFive()
    {
        // Arrange
        var department = await InsertDepartmentAsync("ロックアウト部署");

        var employee = await InsertEmployeeAsync(
            department,
            "ロックアウト乃片野",
            "ロックアウトノ片野");

        var account = await InsertEmployeeAccountAsync(
            employee,
            accessFailedCount: 4);

        _context.ChangeTracker.Clear();

        // Act
        await _repository.RecordLoginFailureAsync(account.Name);

        // Assert
        var savedAccount = await _context.EmployeeAccounts
            .AsNoTracking()
            .SingleAsync(saved =>
                saved.AccountUuid == account.AccountUuid);

        Assert.AreEqual(5, savedAccount.AccessFailedCount);
        Assert.IsNotNull(savedAccount.LockoutEnd);
    }

    [TestMethod(DisplayName = "UT-REP-EAC-012 ログイン成功時に失敗回数とロックアウト時刻をリセットできる")]
    public async Task ResetLoginFailureAsync_ShouldResetAccessFailedCountAndLockoutEnd()
    {
        // Arrange
        var department = await InsertDepartmentAsync("リセット部署");

        var employee = await InsertEmployeeAsync(
            department,
            "リセット乃片野",
            "リセットノカタノ");

        var account = await InsertEmployeeAccountAsync(
            employee,
            accessFailedCount: 5,
            lockoutEnd: DateTime.UtcNow.AddMinutes(10));

        _context.ChangeTracker.Clear();

        // Act
        await _repository.ResetLoginFailureAsync(account.Name);

        // Assert
        var savedAccount = await _context.EmployeeAccounts
            .AsNoTracking()
            .SingleAsync(saved =>
                saved.AccountUuid == account.AccountUuid);

        Assert.AreEqual(0, savedAccount.AccessFailedCount);
        Assert.IsNull(savedAccount.LockoutEnd);
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
        EmployeeEntity employee,
        int accessFailedCount = 0,
        DateTime? lockoutEnd = null)
    {
        var account = new EmployeeAccountEntity
        {
            AccountUuid = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Employee = employee,
            Name = CreateUniqueAccountName(),
            Password = "hashvalue123",
            LockoutEnd = lockoutEnd,
            AccessFailedCount = accessFailedCount
        };

        _accountUuids.Add(account.AccountUuid);

        await _context.EmployeeAccounts.AddAsync(account);
        await _context.SaveChangesAsync();

        return account;
    }

    private static EmployeeAccount CreateEmployeeAccountDomain(
        EmployeeEntity employee,
        string accountName)
    {
        var department = new Department(
            employee.Department.DepartmentUuid.ToString(),
            employee.Department.Name);

        var employeeDomain = new Employee(
            employee.EmployeeUuid.ToString(),
            employee.Name,
            employee.Kana,
            department);

        return new EmployeeAccount(
            Guid.NewGuid().ToString(),
            accountName,
            "pass123",
            "hashvalue123",
            employeeDomain,
            null,
            0);
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