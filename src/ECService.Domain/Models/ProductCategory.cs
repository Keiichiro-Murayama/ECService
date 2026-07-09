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
    /// DB定義の VARCHAR(30) に対応
    /// </summary>
    private const int NameMaxLength = 30;

    /// <summary>
    /// コンストラクタ
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
    /// 商品カテゴリ名を検証する
    /// </summary>
    public static void ValidateName(string name)
    {
        if (name.Length > NameMaxLength)
        {
            throw new DomainException("商品カテゴリ名は30文字以内で入力してください", nameof(name));
        }
    }
}