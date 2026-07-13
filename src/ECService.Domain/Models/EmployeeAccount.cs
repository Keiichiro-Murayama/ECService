using System.Text.RegularExpressions;
using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>。
/// 社員アカウントを表すドメインオブジェクト
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
    /// 入力されたパスワード
    /// 画面入力値のバリデーション用
    /// DBには保存しない
    /// </summary>
    public string Password { get; private set; }

    /// <summary>
    /// ハッシュ化済みパスワード
    /// DBに保存する値
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
    /// アカウント名・パスワードの最小文字数
    /// </summary>
    private const int MinLength = 5;

    /// <summary>
    /// アカウント名・パスワードの最大文字数
    /// </summary>
    private const int MaxLength = 20;

    /// <summary>
    /// ハッシュ化済みパスワードの最大文字数
    /// DB定義の VARCHAR(255) に対応
    /// </summary>
    private const int PasswordHashMaxLength = 255;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public EmployeeAccount(
        string accountUuid,
        string accountName,
        string password,
        string passwordHash,
        Employee employee)
    {
        ValidateUuid(accountUuid);
        ValidateAccountName(accountName);
        ValidatePassword(password);
        ValidatePasswordHash(passwordHash);
        ValidateEmployee(employee);

        AccountUuid = accountUuid;
        AccountName = accountName;
        Password = password;
        PasswordHash = passwordHash;
        Employee = employee;
    }

    /// <summary>
    /// 社員アカウントを生成する
    /// </summary>
    public static EmployeeAccount Create(
        string name,
        string password,
        string passwordHash, //石原:passwordHashを引数に追加
        Employee employee)
    {
        ValidateAccountName(name);
        ValidatePassword(password);
        ValidatePasswordHash(passwordHash);
        ValidateEmployee(employee);

        var accountUuid = Guid.NewGuid().ToString();

        return new EmployeeAccount(
            accountUuid,
            name,
            password,
            passwordHash,
            employee);
    }

    /// <summary>
    /// パスワードを変更する
    /// </summary>
    public void ChangePassword(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// アカウント名を検証する
    /// </summary>
    public static void ValidateAccountName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("アカウント名を入力してください", nameof(name));
        }

        if (name.Length < MinLength || name.Length > MaxLength)
        {
            throw new DomainException("アカウント名は5～20文字で入力してください", nameof(name));
        }

        if (!Regex.IsMatch(name, @"^[a-zA-Z0-9]+$"))
        {
            throw new DomainException("アカウント名は半角英数字で入力してください", nameof(name));
        }
        // 全て同じ文字で構成されていないか
        if (name.All(c => c == name[0]))
        {
            throw new DomainException(
                "アカウント名は同じ文字だけで入力できません。",
                nameof(name));
        }
    }

    /// <summary>
    /// 入力されたパスワードを検証する
    /// </summary>
    public static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("パスワードを入力してください", nameof(password));
        }

        if (password.Length < MinLength || password.Length > MaxLength)
        {
            throw new DomainException("パスワードは5～20文字で入力してください", nameof(password));
        }

        if (!Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
        {
            throw new DomainException("パスワードは半角英数字で入力してください", nameof(password));
        }
        // 全て同じ文字で構成されていないか
        if (password.All(c => c == password[0]))
        {
            throw new DomainException(
                "パスワードは同じ文字だけで入力できません。",
                nameof(password));
        }
    }

    /// <summary>
    /// ハッシュ化済みパスワードを検証する
    /// </summary>
    public static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("パスワードハッシュは必須です。", nameof(passwordHash));
        }

        if (passwordHash.Length > PasswordHashMaxLength)
        {
            throw new DomainException("パスワードハッシュは255文字以内で指定してください。", nameof(passwordHash));
        }
    }

    /// <summary>
    /// 社員を検証する
    /// </summary>
    private static void ValidateEmployee(Employee employee)
    {
        if (employee is null)
        {
            throw new DomainException("社員名を選択してください", nameof(employee));
        }
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string accountUuid)
    {
        if (string.IsNullOrWhiteSpace(accountUuid))
        {
            throw new DomainException("識別Idは必須です。", nameof(accountUuid));
        }

        if (!Guid.TryParse(accountUuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", nameof(accountUuid));
        }
    }
}