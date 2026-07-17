using System;
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
    /// DomainExceptionのメッセージとパラメータ名を確認する
    /// </summary>
    private static void AssertDomainException(
        Action action,
        string expectedMessage,
        string expectedParamName)
    {
        var exception =
            Assert.ThrowsExactly<DomainException>(action);

        Assert.AreEqual(
            expectedMessage,
            exception.Message);

        Assert.AreEqual(
            expectedParamName,
            exception.ParamName);
    }

    /// <summary>
    /// テスト用のEmployeeを生成する
    /// </summary>
    private static Employee CreateEmployee()
    {
        return (Employee)RuntimeHelpers.GetUninitializedObject(
            typeof(Employee));
    }

    /// <summary>
    /// テスト用の社員アカウントを生成する
    /// </summary>
    private static EmployeeAccount CreateEmployeeAccount()
    {
        return EmployeeAccount.Create(
            "user01",
            "Pass123",
            "hashed-password-value",
            CreateEmployee());
    }

    // =========================================================
    // Create 正常系
    // =========================================================

    [TestMethod]
    public void Create_正常な値を渡した場合_社員アカウントが生成される()
    {
        // Arrange
        const string accountName = "user01";
        const string password = "Pass123";
        const string passwordHash = "hashed-password-value";

        var employee = CreateEmployee();

        // Act
        var employeeAccount =
            EmployeeAccount.Create(
                accountName,
                password,
                passwordHash,
                employee);

        // Assert
        Assert.IsFalse(
            string.IsNullOrWhiteSpace(employeeAccount.AccountUuid));

        Assert.IsTrue(
            Guid.TryParse(employeeAccount.AccountUuid, out _));

        Assert.AreEqual(
            accountName,
            employeeAccount.AccountName);

        Assert.AreEqual(
            password,
            employeeAccount.Password);

        Assert.AreEqual(
            passwordHash,
            employeeAccount.PasswordHash);

        Assert.AreSame(
            employee,
            employeeAccount.Employee);

        Assert.IsNull(
            employeeAccount.LockoutEnd);

        Assert.AreEqual(
            0,
            employeeAccount.AccessFailedCount);
    }

    // =========================================================
    // Create アカウント名
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
        var accountName =
            new string('a', length);

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
        var accountName =
            "a" + new string('b', length - 1);

        // Act
        var employeeAccount =
            EmployeeAccount.Create(
                accountName,
                "Pass123",
                "hashed-password-value",
                CreateEmployee());

        // Assert
        Assert.AreEqual(
            accountName,
            employeeAccount.AccountName);
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
    // Create パスワード
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
        var password =
            new string('a', length);

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
        var password =
            "a" + new string('b', length - 1);

        // Act
        var employeeAccount =
            EmployeeAccount.Create(
                "user01",
                password,
                "hashed-password-value",
                CreateEmployee());

        // Assert
        Assert.AreEqual(
            password,
            employeeAccount.Password);
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
    // Create パスワードハッシュ
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
        var passwordHash =
            new string('a', 255);

        // Act
        var employeeAccount =
            EmployeeAccount.Create(
                "user01",
                "Pass123",
                passwordHash,
                CreateEmployee());

        // Assert
        Assert.AreEqual(
            255,
            employeeAccount.PasswordHash.Length);

        Assert.AreEqual(
            passwordHash,
            employeeAccount.PasswordHash);
    }

    [TestMethod]
    public void Create_パスワードハッシュが256文字の場合_DomainExceptionが発生する()
    {
        // Arrange
        var passwordHash =
            new string('a', 256);

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
    // Create Employee
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
    // ChangePassword 正常系
    // =========================================================

    [TestMethod]
    public void ChangePassword_正常なパスワードハッシュを渡した場合_ハッシュが変更される()
    {
        // Arrange
        var employeeAccount =
            CreateEmployeeAccount();

        const string newPasswordHash =
            "new-hashed-password";

        // Act
        employeeAccount.ChangePassword(
            newPasswordHash);

        // Assert
        Assert.AreEqual(
            newPasswordHash,
            employeeAccount.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_実行した場合_入力用パスワードには影響しない()
    {
        // Arrange
        var employeeAccount =
            CreateEmployeeAccount();

        var originalPassword =
            employeeAccount.Password;

        // Act
        employeeAccount.ChangePassword(
            "new-hashed-password");

        // Assert
        Assert.AreEqual(
            originalPassword,
            employeeAccount.Password);
    }

    // =========================================================
    // ChangePassword 異常系
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void ChangePassword_パスワードハッシュが未入力の場合_DomainExceptionが発生する(
        string? passwordHash)
    {
        // Arrange
        var employeeAccount =
            CreateEmployeeAccount();

        var originalPasswordHash =
            employeeAccount.PasswordHash;

        // Act & Assert
        AssertDomainException(
            () => employeeAccount.ChangePassword(
                passwordHash!),
            "パスワードハッシュは必須です。",
            "passwordHash");

        Assert.AreEqual(
            originalPasswordHash,
            employeeAccount.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_パスワードハッシュが256文字の場合_例外が発生して元の値が保持される()
    {
        // Arrange
        var employeeAccount =
            CreateEmployeeAccount();

        var originalPasswordHash =
            employeeAccount.PasswordHash;

        var newPasswordHash =
            new string('a', 256);

        // Act & Assert
        AssertDomainException(
            () => employeeAccount.ChangePassword(
                newPasswordHash),
            "パスワードハッシュは255文字以内で指定してください。",
            "passwordHash");

        Assert.AreEqual(
            originalPasswordHash,
            employeeAccount.PasswordHash);
    }

    [TestMethod]
    public void ChangePassword_パスワードハッシュが255文字の場合_変更される()
    {
        // Arrange
        var employeeAccount =
            CreateEmployeeAccount();

        var newPasswordHash =
            new string('a', 255);

        // Act
        employeeAccount.ChangePassword(
            newPasswordHash);

        // Assert
        Assert.AreEqual(
            255,
            employeeAccount.PasswordHash.Length);

        Assert.AreEqual(
            newPasswordHash,
            employeeAccount.PasswordHash);
    }
}