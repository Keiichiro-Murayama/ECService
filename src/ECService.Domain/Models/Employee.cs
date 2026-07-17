using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 社員を表すドメインオブジェクト
/// Departmentを参照として保持する
/// </summary>
public class Employee : Entity
{
    /// <summary>
    /// 社員UUID
    /// </summary>
    public string EmployeeUuid { get; private set; }

    /// <summary>
    /// 社員名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 社員名カナ
    /// </summary>
    public string Kana { get; private set; }

    /// <summary>
    /// 所属部署
    /// </summary>
    public Department Department { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => EmployeeUuid;

    /// <summary>
    /// 社員名の最大文字数
    /// DB定義の VARCHAR(100) に対応
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// 社員名カナの最大文字数
    /// DB定義の VARCHAR(100) に対応
    /// </summary>
    private const int KanaMaxLength = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Employee(
        string employeeUuid,
        string name,
        string kana,
        Department department)
    {
        ValidateEmployeeUuid(employeeUuid);
        ValidateName(name);
        ValidateKana(kana);
        ValidateDepartment(department);

        EmployeeUuid = employeeUuid;
        Name = name;
        Kana = kana;
        Department = department;
    }

    /// <summary>
    /// 社員UUIDを検証する
    /// </summary>
    private static void ValidateEmployeeUuid(string employeeUuid)
    {
        if (string.IsNullOrWhiteSpace(employeeUuid))
        {
            throw new DomainException("識別Idは必須です。", nameof(employeeUuid));
        }

        if (!Guid.TryParse(employeeUuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", nameof(employeeUuid));
        }
    }

    /// <summary>
    /// 社員名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("社員名を選択してください", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException("社員名は100文字以内で入力してください", nameof(name));
        }
    }

    /// <summary>
    /// 社員名カナを検証する
    /// </summary>
    private static void ValidateKana(string kana)
    {
        if (string.IsNullOrWhiteSpace(kana))
        {
            throw new DomainException("社員名を選択してください", nameof(kana));
        }

        if (kana.Length > KanaMaxLength)
        {
            throw new DomainException("社員名カナは100文字以内で入力してください", nameof(kana));
        }
    }

    /// <summary>
    /// 部署を検証する
    /// </summary>
    private static void ValidateDepartment(Department department)
    {
        if (department is null)
        {
            throw new DomainException("社員名を選択してください", nameof(department));
        }
    }
}