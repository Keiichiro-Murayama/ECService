using System.Runtime.CompilerServices;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Domain.Tests.Models;

/// <summary>
/// EmployeeAccountドメインオブジェクトの単体テスト
/// </summary>
[TestClass]
public class EmployeeAccountTests
{
    /// <summary>
    /// テスト用の社員を生成する
    /// Employeeのコンストラクタに依存せず、
    /// EmployeeAccountのテスト対象だけを確認する
    /// </summary>
    private static Employee CreateEmployee()
    {
        return (Employee)RuntimeHelpers.GetUninitializedObject(
            typeof(Employee));
    }

    /// <summary>
    /// 指定された文字数の正常な半角英数字を生成する
    /// 末尾だけ異なる文字にして、同一文字チェックを通過させる
    /// </summary>
    private static string CreateValidAlphaNumeric(int length)
    {
        return new string('a', length - 1) + "1";
    }

    /// <summary>
    /// テスト用の社員アカウントを復元する
    /// </summary>
    private static EmployeeAccount CreateEmployeeAccount(
        string accountUuid = "11111111-1111-1111-1111-111111111111",
        string accountName = "user01",
        string passwordHash = "hashed-password-value",
        DateTime? lockoutEnd = null,
        int accessFailedCount = 0)
    {
        return new EmployeeAccount(
            accountUuid,
            accountName,
            passwordHash,
            CreateEmployee(),
            lockoutEnd,
            accessFailedCount);
    }

    /// <summary>
    /// DomainExceptionの内容を確認する
    /// </summary>
    private static void AssertDomainException(
        Action action,
        string expectedMessage,
        string expectedParamName)
    {
        var exception =
            Assert.ThrowsExactly<DomainException>(action);

        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(expectedParamName, exception.ParamName);
    }

    // =========================================================
    // Create
    // =========================================================

    [TestMethod]
    public void Create_正常な値を渡した場合_社員アカウントが生成される()
    {
        // Arrange
        var employee = CreateEmployee();

        // Act
        var account = EmployeeAccount.Create(
            "user01",
            "Pass123",
            "hashed-password-value",
            employee);

        // Assert
        Assert.IsFalse(
            string.IsNullOrWhiteSpace(account.AccountUuid));

        Assert.IsTrue(
            Guid.TryParse(account.AccountUuid, out _));

        Assert.AreEqual("user01", account.AccountName);
        Assert.AreEqual("Pass123", account.Password);
        Assert.AreEqual(
            "hashed-password-value",
            account.PasswordHash);

        Assert.AreSame(employee, account.Employee);
        Assert.IsNull(account.LockoutEnd);
        Assert.AreEqual(0, account.AccessFailedCount);
    }

    // =========================================================
    // アカウント名
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_アカウント名が未入力の場合_DomainExceptionが発生する(
        string? accountName)
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                accountName!,
                "Pass123",
                "hashed-password-value",
                CreateEmployee()),
            "アカウント名を入力してください",
            "name");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(21)]
    public void Create_アカウント名が文字数範囲外の場合_DomainExceptionが発生する(
        int length)
    {
        // Arrange
        var accountName = CreateValidAlphaNumeric(length);

        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                accountName,
                "Pass123",
                "hashed-password-value",
                CreateEmployee()),
            "アカウント名は5～20文字で入力してください",
            "name");
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(20)]
    public void Create_アカウント名が境界値の場合_社員アカウントが生成される(
        int length)
    {
        // Arrange
        var accountName = CreateValidAlphaNumeric(length);

        // Act
        var account = EmployeeAccount.Create(
            accountName,
            "Pass123",
            "hashed-password-value",
            CreateEmployee());

        // Assert
        Assert.AreEqual(accountName, account.AccountName);
    }

    [TestMethod]
    [DataRow("abc_1")]
    [DataRow("abc-1")]
    [DataRow("ＡＢＣ12")]
    public void Create_アカウント名が半角英数字以外の場合_DomainExceptionが発生する(
        string accountName)
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                accountName,
                "Pass123",
                "hashed-password-value",
                CreateEmployee()),
            "アカウント名は半角英数字で入力してください",
            "name");
    }

    [TestMethod]
    public void Create_アカウント名が同じ文字だけの場合_DomainExceptionが発生する()
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "aaaaa",
                "Pass123",
                "hashed-password-value",
                CreateEmployee()),
            "アカウント名は同じ文字だけで入力できません。",
            "name");
    }

    // =========================================================
    // パスワード
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_パスワードが未入力の場合_DomainExceptionが発生する(
        string? password)
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                password!,
                "hashed-password-value",
                CreateEmployee()),
            "パスワードを入力してください",
            "password");
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(21)]
    public void Create_パスワードが文字数範囲外の場合_DomainExceptionが発生する(
        int length)
    {
        // Arrange
        var password = CreateValidAlphaNumeric(length);

        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                password,
                "hashed-password-value",
                CreateEmployee()),
            "パスワードは5～20文字で入力してください",
            "password");
    }

    [TestMethod]
    [DataRow(5)]
    [DataRow(20)]
    public void Create_パスワードが境界値の場合_社員アカウントが生成される(
        int length)
    {
        // Arrange
        var password = CreateValidAlphaNumeric(length);

        // Act
        var account = EmployeeAccount.Create(
            "user01",
            password,
            "hashed-password-value",
            CreateEmployee());

        // Assert
        Assert.AreEqual(password, account.Password);
    }

    [TestMethod]
    [DataRow("abc_1")]
    [DataRow("abc-1")]
    [DataRow("ＡＢＣ12")]
    public void Create_パスワードが半角英数字以外の場合_DomainExceptionが発生する(
        string password)
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                password,
                "hashed-password-value",
                CreateEmployee()),
            "パスワードは半角英数字で入力してください",
            "password");
    }

    [TestMethod]
    public void Create_パスワードが同じ文字だけの場合_DomainExceptionが発生する()
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                "11111",
                "hashed-password-value",
                CreateEmployee()),
            "パスワードは同じ文字だけで入力できません。",
            "password");
    }

    // =========================================================
    // パスワードハッシュ
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_パスワードハッシュが未入力の場合_DomainExceptionが発生する(
        string? passwordHash)
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                "Pass123",
                passwordHash!,
                CreateEmployee()),
            "パスワードハッシュは必須です。",
            "passwordHash");
    }

    [TestMethod]
    public void Create_パスワードハッシュが255文字の場合_社員アカウントが生成される()
    {
        // Arrange
        var passwordHash = new string('h', 255);

        // Act
        var account = EmployeeAccount.Create(
            "user01",
            "Pass123",
            passwordHash,
            CreateEmployee());

        // Assert
        Assert.AreEqual(passwordHash, account.PasswordHash);
        Assert.AreEqual(255, account.PasswordHash.Length);
    }

    [TestMethod]
    public void Create_パスワードハッシュが256文字の場合_DomainExceptionが発生する()
    {
        // Arrange
        var passwordHash = new string('h', 256);

        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                "Pass123",
                passwordHash,
                CreateEmployee()),
            "パスワードハッシュは255文字以内で指定してください。",
            "passwordHash");
    }

    // =========================================================
    // 社員
    // =========================================================

    [TestMethod]
    public void Create_社員がnullの場合_DomainExceptionが発生する()
    {
        // Act & Assert
        AssertDomainException(
            () => EmployeeAccount.Create(
                "user01",
                "Pass123",
                "hashed-password-value",
                null!),
            "社員名を選択してください",
            "employee");
    }

    // =========================================================
    // 7引数コンストラクタ
    // =========================================================

    [TestMethod]
    public void コンストラクタ_正常な値を渡した場合_指定された状態で生成される()
    {
        // Arrange
        const string accountUuid =
            "22222222-2222-2222-2222-222222222222";

        var employee = CreateEmployee();

        var lockoutEnd = new DateTime(
            2026,
            7,
            20,
            12,
            0,
            0,
            DateTimeKind.Utc);

        // Act
        var account = new EmployeeAccount(
            accountUuid,
            "employee01",
            "Pass123",
            "hashed-password-value",
            employee,
            lockoutEnd,
            2);

        // Assert
        Assert.AreEqual(accountUuid, account.AccountUuid);
        Assert.AreEqual("employee01", account.AccountName);
        Assert.AreEqual("Pass123", account.Password);
        Assert.AreEqual(
            "hashed-password-value",
            account.PasswordHash);

        Assert.AreSame(employee, account.Employee);
        Assert.AreEqual(lockoutEnd, account.LockoutEnd);
        Assert.AreEqual(2, account.AccessFailedCount);
    }

    // =========================================================
    // UUID
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void コンストラクタ_UUIDが未入力の場合_DomainExceptionが発生する(
        string? accountUuid)
    {
        // Act & Assert
        AssertDomainException(
            () => new EmployeeAccount(
                accountUuid!,
                "user01",
                "Pass123",
                "hashed-password-value",
                CreateEmployee(),
                null,
                0),
            "識別Idは必須です。",
            "accountUuid");
    }

    [TestMethod]
    [DataRow("invalid-guid")]
    [DataRow("12345")]
    public void コンストラクタ_UUIDの形式が不正な場合_DomainExceptionが発生する(
        string accountUuid)
    {
        // Act & Assert
        AssertDomainException(
            () => new EmployeeAccount(
                accountUuid,
                "user01",
                "Pass123",
                "hashed-password-value",
                CreateEmployee(),
                null,
                0),
            "識別Idの形式が不正です。",
            "accountUuid");
    }

    // =========================================================
    // 復元用6引数コンストラクタ
    // =========================================================

    [TestMethod]
    public void 復元用コンストラクタ_正常な値を渡した場合_平文パスワードなしで復元される()
    {
        // Arrange
        const string accountUuid =
            "33333333-3333-3333-3333-333333333333";

        var employee = CreateEmployee();

        var lockoutEnd = new DateTime(
            2026,
            7,
            25,
            10,
            30,
            0,
            DateTimeKind.Utc);

        // Act
        var account = new EmployeeAccount(
            accountUuid,
            "admin01",
            "restored-hash-value",
            employee,
            lockoutEnd,
            3);

        // Assert
        Assert.AreEqual(accountUuid, account.AccountUuid);
        Assert.AreEqual("admin01", account.AccountName);
        Assert.AreEqual(string.Empty, account.Password);
        Assert.AreEqual(
            "restored-hash-value",
            account.PasswordHash);

        Assert.AreSame(employee, account.Employee);
        Assert.AreEqual(lockoutEnd, account.LockoutEnd);
        Assert.AreEqual(3, account.AccessFailedCount);
    }

    // =========================================================
    // 4引数コンストラクタ
    // =========================================================

    [TestMethod]
    public void 四引数コンストラクタ_正常な値を渡した場合_プロパティが設定される()
    {
        // Arrange
        const string accountUuid =
            "44444444-4444-4444-4444-444444444444";

        var employee = CreateEmployee();

        // Act
        var account = new EmployeeAccount(
            accountUuid,
            "staff01",
            "four-argument-hash",
            employee);

        // Assert
        Assert.AreEqual(accountUuid, account.AccountUuid);
        Assert.AreEqual("staff01", account.AccountName);
        Assert.AreEqual(string.Empty, account.Password);
        Assert.AreEqual(
            "four-argument-hash",
            account.PasswordHash);

        Assert.AreSame(employee, account.Employee);
        Assert.IsNull(account.LockoutEnd);
        Assert.AreEqual(0, account.AccessFailedCount);
    }

    // =========================================================
    // ChangePassword
    // =========================================================

    [TestMethod]
    public void ChangePassword_正常なパスワードハッシュを渡した場合_ハッシュが変更される()
    {
        // Arrange
        var account = CreateEmployeeAccount();

        // Act
        account.ChangePassword("new-hashed-password");

        // Assert
        Assert.AreEqual(
            "new-hashed-password",
            account.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_実行した場合_入力用パスワードには影響しない()
    {
        // Arrange
        var account = new EmployeeAccount(
            "55555555-5555-5555-5555-555555555555",
            "user01",
            "Pass123",
            "old-hash",
            CreateEmployee(),
            null,
            0);

        // Act
        account.ChangePassword("new-hash");

        // Assert
        Assert.AreEqual("Pass123", account.Password);
        Assert.AreEqual("new-hash", account.PasswordHash);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void ChangePassword_パスワードハッシュが未入力の場合_DomainExceptionが発生する(
        string? passwordHash)
    {
        // Arrange
        var account = CreateEmployeeAccount();
        var originalHash = account.PasswordHash;

        // Act & Assert
        AssertDomainException(
            () => account.ChangePassword(passwordHash!),
            "パスワードハッシュは必須です。",
            "passwordHash");

        Assert.AreEqual(originalHash, account.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_パスワードハッシュが256文字の場合_例外が発生して元の値が保持される()
    {
        // Arrange
        var account = CreateEmployeeAccount();
        var originalHash = account.PasswordHash;
        var passwordHash = new string('h', 256);

        // Act & Assert
        AssertDomainException(
            () => account.ChangePassword(passwordHash),
            "パスワードハッシュは255文字以内で指定してください。",
            "passwordHash");

        Assert.AreEqual(originalHash, account.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_パスワードハッシュが255文字の場合_変更される()
    {
        // Arrange
        var account = CreateEmployeeAccount();
        var passwordHash = new string('h', 255);

        // Act
        account.ChangePassword(passwordHash);

        // Assert
        Assert.AreEqual(passwordHash, account.PasswordHash);
    }

    // =========================================================
    // Entityの同一性
    // =========================================================

    [TestMethod]
    public void Equals_AccountUuidが同じ場合_同一の社員アカウントと判定される()
    {
        // Arrange
        const string accountUuid =
            "66666666-6666-6666-6666-666666666666";

        var first = new EmployeeAccount(
            accountUuid,
            "user01",
            "first-hash",
            CreateEmployee(),
            null,
            0);

        var second = new EmployeeAccount(
            accountUuid,
            "admin01",
            "second-hash",
            CreateEmployee(),
            DateTime.UtcNow.AddDays(1),
            3);

        // Act & Assert
        Assert.IsTrue(first.Equals(second));
        Assert.IsTrue(first == second);

        Assert.AreEqual(
            first.GetHashCode(),
            second.GetHashCode());
    }

    [TestMethod]
    public void Equals_AccountUuidが異なる場合_別の社員アカウントと判定される()
    {
        // Arrange
        var first = new EmployeeAccount(
            "77777777-7777-7777-7777-777777777777",
            "user01",
            "first-hash",
            CreateEmployee(),
            null,
            0);

        var second = new EmployeeAccount(
            "88888888-8888-8888-8888-888888888888",
            "user01",
            "first-hash",
            CreateEmployee(),
            null,
            0);

        // Act & Assert
        Assert.IsFalse(first.Equals(second));
        Assert.IsTrue(first != second);
    }
}