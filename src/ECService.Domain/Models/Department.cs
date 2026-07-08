using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 部署を表すドメインオブジェクト
/// </summary>
public class Department : Entity
{
    /// <summary>
    /// 部署UUID
    /// </summary>
    public string DepartmentUuid { get; private set; }

    /// <summary>
    /// 部署名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => DepartmentUuid;

    /// <summary>
    /// 部署名の最大文字数
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Department(string departmentUuid, string name)
    {
        DepartmentUuid = departmentUuid;
        Name = name;
    }

    /// <summary>
    /// 新しい部署を生成する
    /// </summary>
    public static Department Create(string name)
    {
        ValidateName(name);

        var departmentUuid = Guid.NewGuid().ToString();

        return new Department(departmentUuid, name);
    }

    /// <summary>
    /// 既存の部署を復元する
    /// </summary>
    public static Department Restore(string departmentUuid, string name)
    {
        ValidateUuid(departmentUuid, nameof(departmentUuid));
        ValidateName(name);

        return new Department(departmentUuid, name);
    }

    /// <summary>
    /// 部署名を変更する
    /// </summary>
    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    /// <summary>
    /// 部署名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("部署名は必須です。", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                $"部署名は{NameMaxLength}文字以内で指定してください。", nameof(name));
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