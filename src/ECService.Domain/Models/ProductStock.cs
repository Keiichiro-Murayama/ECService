using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 商品在庫を表すドメインオブジェクト
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
    /// 在庫数の最大値
    /// </summary>
    private const int QuantityMaxValue = 1000;

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

public static ProductStock Restore(string stockUuid,int quantity)
    {
        ValidateQuantity(quantity);

        return new ProductStock(stockUuid,quantity);
    }

    /// <summary>
    /// 在庫数を検証する
    /// </summary>
    public static void ValidateQuantity(int quantity)
    {
        if (quantity > QuantityMaxValue)
        {
            throw new DomainException("在庫数は1000個以下で入力してください", nameof(quantity));
        }
    }

    /// <summary>
    /// 在庫数を変更する
    /// </summary>
    public void ChangeQuantity(int quantity)
    {
        ValidateQuantity(quantity);

        Quantity = quantity;
    }
}