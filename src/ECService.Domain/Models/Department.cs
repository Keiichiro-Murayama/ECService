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
    /// DB定義の VARCHAR(100) に対応
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Department(string departmentUuid, string name)
    {
        ValidateDepartmentUuid(departmentUuid);
        ValidateName(name);

        DepartmentUuid = departmentUuid;
        Name = name;
    }

    /// <summary>
    /// 部署UUIDを検証する
    /// </summary>
    private static void ValidateDepartmentUuid(string departmentUuid)
    {
        if (string.IsNullOrWhiteSpace(departmentUuid))
        {
            throw new DomainException("識別Idは必須です。", nameof(departmentUuid));
        }

        if (!Guid.TryParse(departmentUuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", nameof(departmentUuid));
        }
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
            throw new DomainException("部署名は100文字以内で入力してください", nameof(name));
        }
    }
}