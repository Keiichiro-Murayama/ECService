using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 社員アカウントを表すドメインオブジェクト
/// Employeeを参照として保持する
/// </summary>
public class EmployeeAccount : Entity
{
    /// <summary>
    /// アカウントUUID
    /// </summary>
    public string AccountUuid { get; private set; }

    /// <summary>
    /// アカウント名
    /// </summary>
    public string AccountName { get; private set; }

    /// <summary>
    /// パスワード
    /// DB上はハッシュ値を保持する
    /// </summary>
    public string PasswordHash { get; private set; }

    /// <summary>
    /// 社員
    /// </summary>
    public Employee Employee { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => AccountUuid;

    /// <summary>
    /// アカウント名の最大文字数
    /// </summary>
    private const int AccountNameMaxLength = 20;

    /// <summary>
    /// パスワードハッシュの最大文字数
    /// </summary>
    private const int PasswordHashMaxLength = 255;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private EmployeeAccount(
        string accountUuid,
        string accountName,
        string passwordHash,
        Employee employee)
    {
        AccountUuid = accountUuid;
        AccountName = accountName;
        PasswordHash = passwordHash;
        Employee = employee;
    }

    /// <summary>
    /// 新しい社員アカウントを生成する
    /// </summary>
    public static EmployeeAccount Create(
        string accountName,
        string passwordHash,
        Employee employee)
    {
        ValidateAccountName(accountName);
        ValidatePasswordHash(passwordHash);
        ValidateEmployee(employee);

        var accountUuid = Guid.NewGuid().ToString();

        return new EmployeeAccount(accountUuid, accountName, passwordHash, employee);
    }

    /// <summary>
    /// 既存の社員アカウントを復元する
    /// </summary>
    public static EmployeeAccount Restore(
        string accountUuid,
        string accountName,
        string passwordHash,
        Employee employee)
    {
        ValidateUuid(accountUuid, nameof(accountUuid));
        ValidateAccountName(accountName);
        ValidatePasswordHash(passwordHash);
        ValidateEmployee(employee);

        return new EmployeeAccount(accountUuid, accountName, passwordHash, employee);
    }

    /// <summary>
    /// アカウント名を変更する
    /// </summary>
    public void ChangeAccountName(string accountName)
    {
        ValidateAccountName(accountName);
        AccountName = accountName;
    }

    /// <summary>
    /// パスワードハッシュを変更する
    /// </summary>
    public void ChangePassword(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// 同じアカウント名か判定する
    /// </summary>
    public bool IsSameAccount(string accountName)
    {
        return AccountName == accountName;
    }

    /// <summary>
    /// アカウント名を検証する
    /// </summary>
    private static void ValidateAccountName(string accountName)
    {
        if (string.IsNullOrWhiteSpace(accountName))
        {
            throw new DomainException("アカウント名は必須です。", nameof(accountName));
        }

        if (accountName.Length > AccountNameMaxLength)
        {
            throw new DomainException(
                $"アカウント名は{AccountNameMaxLength}文字以内で指定してください。", nameof(accountName));
        }
    }

    /// <summary>
    /// パスワードハッシュを検証する
    /// </summary>
    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("パスワードは必須です。", nameof(passwordHash));
        }

        if (passwordHash.Length > PasswordHashMaxLength)
        {
            throw new DomainException(
                $"パスワードは{PasswordHashMaxLength}文字以内で指定してください。", nameof(passwordHash));
        }
    }

    /// <summary>
    /// 社員を検証する
    /// </summary>
    private static void ValidateEmployee(Employee employee)
    {
        if (employee is null)
        {
            throw new DomainException("社員は必須です。", nameof(employee));
        }
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string uuid, string paramName)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            throw new DomainException("識別Idは必須です。", paramName);
        }

        if (!Guid.TryParse(uuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", paramName);
        }
    }
}