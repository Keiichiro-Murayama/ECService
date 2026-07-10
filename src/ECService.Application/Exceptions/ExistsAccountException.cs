namespace ECService.Application.Exceptions;
/// <summary>
/// 内部エラーを表す例外クラス
/// </summary>
public class ExistsAccountException : Exception
{
    public ExistsAccountException(string message) : 
    base(message) { }
    public ExistsAccountException(string message, Exception innerException) : 
    base(message, innerException) { }
}