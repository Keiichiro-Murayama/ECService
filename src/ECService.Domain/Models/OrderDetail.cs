using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 注文明細を表すドメインオブジェクト
/// </summary>
public class OrderDetail : Entity
{
    /// <summary>
    /// 注文明細ID
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// 商品名
    /// </summary>
    public string ProductName { get; private set; }

    /// <summary>
    /// 注文数
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => Id.ToString();

    private OrderDetail(
        int id,
        string productName,
        int count)
    {
        Id = id;
        ProductName = productName;
        Count = count;
    }

    /// <summary>
    /// DBの情報から注文明細を復元する
    /// </summary>
    public static OrderDetail Restore(
        int id,
        string productName,
        int count)
    {
        ValidateId(id);
        ValidateProductName(productName);
        ValidateCount(count);

        return new OrderDetail(
            id,
            productName,
            count);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new DomainException(
                "注文明細IDは1以上である必要があります。",
                nameof(id));
        }
    }

    private static void ValidateProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException(
                "商品名は必須です。",
                nameof(productName));
        }
    }

    private static void ValidateCount(int count)
    {
        if (count <= 0)
        {
            throw new DomainException(
                "注文数は1個以上である必要があります。",
                nameof(count));
        }
    }
}