namespace ECService.Domain.Exceptions;
/// <summary>
/// 業務ルール違反を表す例外クラス
/// </summary>
public class InternalException : Exception
{
    public InternalException(string message) : base(message) { }
    public InternalException(string message, Exception innerException)
    : base(message, innerException) { }
}