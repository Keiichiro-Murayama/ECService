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
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// 社員名カナの最大文字数
    /// </summary>
    private const int KanaMaxLength = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Employee(
        string employeeUuid,
        string name,
        string kana,
        Department department)
    {
        EmployeeUuid = employeeUuid;
        Name = name;
        Kana = kana;
        Department = department;
    }

    /// <summary>
    /// 新しい社員を生成する
    /// </summary>
    public static Employee Create(
        string name,
        string kana,
        Department department)
    {
        ValidateName(name);
        ValidateKana(kana);
        ValidateDepartment(department);

        var employeeUuid = Guid.NewGuid().ToString();

        return new Employee(employeeUuid, name, kana, department);
    }

    /// <summary>
    /// 既存の社員を復元する
    /// </summary>
    public static Employee Restore(
        string employeeUuid,
        string name,
        string kana,
        Department department)
    {
        ValidateUuid(employeeUuid, nameof(employeeUuid));
        ValidateName(name);
        ValidateKana(kana);
        ValidateDepartment(department);

        return new Employee(employeeUuid, name, kana, department);
    }

    /// <summary>
    /// 社員名を変更する
    /// </summary>
    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    /// <summary>
    /// 社員名カナを変更する
    /// </summary>
    public void ChangeKana(string kana)
    {
        ValidateKana(kana);
        Kana = kana;
    }

    /// <summary>
    /// 所属部署を変更する
    /// </summary>
    public void ChangeDepartment(Department department)
    {
        ValidateDepartment(department);
        Department = department;
    }

    /// <summary>
    /// 社員名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("社員名は必須です。", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                $"社員名は{NameMaxLength}文字以内で指定してください。", nameof(name));
        }
    }

    /// <summary>
    /// 社員名カナを検証する
    /// </summary>
    private static void ValidateKana(string kana)
    {
        if (string.IsNullOrWhiteSpace(kana))
        {
            throw new DomainException("社員名カナは必須です。", nameof(kana));
        }

        if (kana.Length > KanaMaxLength)
        {
            throw new DomainException(
                $"社員名カナは{KanaMaxLength}文字以内で指定してください。", nameof(kana));
        }
    }

    /// <summary>
    /// 部署を検証する
    /// </summary>
    private static void ValidateDepartment(Department department)
    {
        if (department is null)
        {
            throw new DomainException("部署は必須です。", nameof(department));
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