namespace ECService.Infrastructure.Exceptions;

/// <summary>
/// インフラストラクチャ層内部で想定外の状態が発生した場合の例外。
/// </summary>
public class InternalException : Exception
{
    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="message">エラーメッセージ。</param>
    public InternalException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="message">エラーメッセージ。</param>
    /// <param name="innerException">内部例外。</param>
    public InternalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}