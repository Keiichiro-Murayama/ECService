using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 注文を表すドメインオブジェクト
/// </summary>
public class Order : Entity
{
    /// <summary>
    /// 注文UUID
    /// </summary>
    public string OrderUuid { get; private set; }

    /// <summary>
    /// 注文日時
    /// </summary>
    public DateTime OrderDate { get; private set; }

    /// <summary>
    /// 合計金額
    /// </summary>
    public int AmountTotal { get; private set; }

    /// <summary>
    /// 注文した顧客
    /// </summary>
    public Customer Customer { get; private set; }

    /// <summary>
    /// 注文ステータス
    /// </summary>
    public OrderStatus OrderStatus { get; private set; }

    /// <summary>
    /// 注文明細一覧
    /// </summary>
    public IReadOnlyList<OrderDetail> OrderDetails => _orderDetails.AsReadOnly();

    private readonly List<OrderDetail> _orderDetails;

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => OrderUuid;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Order(
        string orderUuid,
        DateTime orderDate,
        int amountTotal,
        Customer customer,
        OrderStatus orderStatus,
        IEnumerable<OrderDetail> orderDetails)
    {
        ValidateOrderUuid(orderUuid);
        ValidateAmountTotal(amountTotal);
        ValidateCustomer(customer);
        ValidateOrderStatus(orderStatus);
        ValidateOrderDetails(orderDetails);

        OrderUuid = orderUuid;
        OrderDate = orderDate;
        AmountTotal = amountTotal;
        Customer = customer;
        OrderStatus = orderStatus;
        _orderDetails = orderDetails.ToList();
    }

    /// <summary>
    /// 注文UUIDを検証する
    /// </summary>
    private static void ValidateOrderUuid(string orderUuid)
    {
        if (string.IsNullOrWhiteSpace(orderUuid))
        {
            throw new DomainException(
                "注文UUIDは必須です。",
                nameof(orderUuid));
        }

        if (!Guid.TryParse(orderUuid, out _))
        {
            throw new DomainException(
                "注文UUIDの形式が不正です。",
                nameof(orderUuid));
        }
    }

    /// <summary>
    /// 合計金額を検証する
    /// </summary>
    private static void ValidateAmountTotal(int amountTotal)
    {
        if (amountTotal < 0)
        {
            throw new DomainException(
                "合計金額は0円以上である必要があります。",
                nameof(amountTotal));
        }
    }

    /// <summary>
    /// 顧客を検証する
    /// </summary>
    private static void ValidateCustomer(Customer customer)
    {
        if (customer is null)
        {
            throw new DomainException(
                "顧客情報は必須です。",
                nameof(customer));
        }
    }

    /// <summary>
    /// 注文ステータスを検証する
    /// </summary>
    private static void ValidateOrderStatus(OrderStatus orderStatus)
    {
        if (orderStatus is null)
        {
            throw new DomainException(
                "注文ステータスは必須です。",
                nameof(orderStatus));
        }
    }

    /// <summary>
    /// 注文明細を検証する
    /// </summary>
    private static void ValidateOrderDetails(
        IEnumerable<OrderDetail> orderDetails)
    {
        if (orderDetails is null)
        {
            throw new DomainException(
                "注文明細は必須です。",
                nameof(orderDetails));
        }

        if (!orderDetails.Any())
        {
            throw new DomainException(
                "注文明細が1件以上必要です。",
                nameof(orderDetails));
        }
    }

    /// <summary>
    /// 注文ステータスを変更する。
    /// </summary>
    /// <param name="newOrderStatus">
    /// 変更後の注文ステータス。
    /// </param>
    public void ChangeOrderStatus(
        OrderStatus newOrderStatus)
    {
        ValidateOrderStatus(newOrderStatus);

        OrderStatus = newOrderStatus;
    }
}