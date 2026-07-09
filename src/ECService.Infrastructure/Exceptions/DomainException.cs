namespace ECService.Infrastructure.Exceptions;

/// <summary>
/// 業務制約を表す例外クラス
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// 違反した項目名
    /// エラーレスポンスの組み立てに利用できる
    /// </summary>
    public string? ParamName { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public DomainException() : base()
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public DomainException(string message, Exception ex) : base(message)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="paramName">違反した項目名</param>
    public DomainException(string message, string paramName) : base(message)
    {
        ParamName = paramName;
    }
}