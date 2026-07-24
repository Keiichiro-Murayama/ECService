using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 注文ステータスを表すドメインオブジェクト
/// </summary>
public class OrderStatus : Entity
{
    /// <summary>
    /// 注文ステータスID
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 注文ステータス名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 注文ステータス名の最大文字数
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => Id.ToString();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OrderStatus(int id, string name)
    {
        ValidateId(id);
        ValidateName(name);

        Id = id;
        Name = name;
    }

    /// <summary>
    /// 注文ステータスIDを検証する
    /// </summary>
    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new DomainException(
                "注文ステータスIDは1以上である必要があります。",
                nameof(id));
        }
    }

    /// <summary>
    /// 注文ステータス名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "注文ステータス名は必須です。",
                nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                "注文ステータス名は100文字以内で入力してください。",
                nameof(name));
        }
    }
}