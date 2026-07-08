using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 商品カテゴリを表すドメインオブジェクト
/// </summary>
public class ProductCategory : Entity
{
    /// <summary>
    /// 商品カテゴリUUID
    /// </summary>
    public string CategoryUuid { get; private set; }

    /// <summary>
    /// 商品カテゴリ名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => CategoryUuid;

    /// <summary>
    /// 商品カテゴリ名の最大文字数
    /// </summary>
    private const int NameMaxLength = 30;

    /// <summary>
    /// コンストラクタ
    /// 生成経路をCreate/Restoreに統一するためprivateにする
    /// </summary>
    private ProductCategory(string categoryUuid, string name)
    {
        CategoryUuid = categoryUuid;
        Name = name;
    }

    /// <summary>
    /// 新しい商品カテゴリを生成する
    /// </summary>
    public static ProductCategory Create(string name)
    {
        ValidateName(name);

        var categoryUuid = Guid.NewGuid().ToString();

        return new ProductCategory(categoryUuid, name);
    }

    /// <summary>
    /// 既存の商品カテゴリを復元する
    /// </summary>
    public static ProductCategory Restore(string categoryUuid, string name)
    {
        ValidateUuid(categoryUuid, nameof(categoryUuid));
        ValidateName(name);

        return new ProductCategory(categoryUuid, name);
    }

    /// <summary>
    /// 商品カテゴリ名を変更する
    /// </summary>
    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    /// <summary>
    /// 同じカテゴリ名か判定する
    /// </summary>
    public bool IsSameName(string name)
    {
        return Name == name;
    }

    /// <summary>
    /// 商品カテゴリ名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("商品カテゴリ名は必須です。", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                $"商品カテゴリ名は{NameMaxLength}文字以内で指定してください。", nameof(name));
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