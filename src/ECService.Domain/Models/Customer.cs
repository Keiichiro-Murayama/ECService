using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 顧客を表すドメインオブジェクト
/// </summary>
public class Customer : Entity
{
    /// <summary>
    /// 顧客UUID
    /// </summary>
    public string CustomerUuid { get; private set; }

    /// <summary>
    /// 顧客アカウント名
    /// </summary>
    public string Username { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => CustomerUuid;

    /// <summary>
    /// 顧客アカウント名の最大文字数
    /// </summary>
    private const int UsernameMaxLength = 30;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Customer(
        string customerUuid,
        string username)
    {
        ValidateCustomerUuid(customerUuid);
        ValidateUsername(username);

        CustomerUuid = customerUuid;
        Username = username;
    }

    /// <summary>
    /// 顧客UUIDを検証する
    /// </summary>
    private static void ValidateCustomerUuid(string customerUuid)
    {
        if (string.IsNullOrWhiteSpace(customerUuid))
        {
            throw new DomainException(
                "顧客UUIDは必須です。",
                nameof(customerUuid));
        }

        if (!Guid.TryParse(customerUuid, out _))
        {
            throw new DomainException(
                "顧客UUIDの形式が不正です。",
                nameof(customerUuid));
        }
    }

    /// <summary>
    /// 顧客アカウント名を検証する
    /// </summary>
    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException(
                "顧客アカウント名は必須です。",
                nameof(username));
        }

        if (username.Length > UsernameMaxLength)
        {
            throw new DomainException(
                "顧客アカウント名は30文字以内で入力してください。",
                nameof(username));
        }
    }
}