namespace ECService.Infrastructure.Exceptions;

/// <summary>
/// 業務制約を表す例外クラス
/// </summary>
public class InternalException : Exception
{
    /// <summary>
    /// 違反した項目名
    /// エラーレスポンスの組み立てに利用できる
    /// </summary>
    public string? ParamName { get; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public InternalException() : base()
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    public InternalException(string message, Exception ex) : base(message)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">エラーメッセージ</param>
    /// <param name="paramName">違反した項目名</param>
    public InternalException(string message, string paramName) : base(message)
    {
        ParamName = paramName;
    }
}