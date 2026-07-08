using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 商品在庫を表すドメインオブジェクト
/// Productに内包される内部エンティティ
/// </summary>
public class ProductStock : Entity
{
    /// <summary>
    /// 商品在庫UUID
    /// </summary>
    public string StockUuid { get; private set; }

    /// <summary>
    /// 商品在庫数
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => StockUuid;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private ProductStock(string stockUuid, int quantity)
    {
        StockUuid = stockUuid;
        Quantity = quantity;
    }

    /// <summary>
    /// 新しい商品在庫を生成する
    /// </summary>
    public static ProductStock Create(int quantity)
    {
        ValidateQuantity(quantity);

        var stockUuid = Guid.NewGuid().ToString();

        return new ProductStock(stockUuid, quantity);
    }

    /// <summary>
    /// 既存の商品在庫を復元する
    /// </summary>
    public static ProductStock Restore(string stockUuid, int quantity)
    {
        ValidateUuid(stockUuid, nameof(stockUuid));
        ValidateQuantity(quantity);

        return new ProductStock(stockUuid, quantity);
    }

    /// <summary>
    /// 在庫数を変更する
    /// </summary>
    public void ChangeQuantity(int quantity)
    {
        ValidateQuantity(quantity);
        Quantity = quantity;
    }

    /// <summary>
    /// 在庫数を検証する
    /// </summary>
    private static void ValidateQuantity(int quantity)
    {
        if (quantity < 0)
        {
            throw new DomainException("商品在庫数は0以上で指定してください。", nameof(quantity));
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