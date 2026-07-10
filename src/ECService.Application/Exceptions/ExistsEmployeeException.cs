namespace ECService.Application.Exceptions;
/// <summary>
/// 内部エラーを表す例外クラス
/// </summary>
public class ExistsEmployeeException : Exception
{
    public ExistsEmployeeException(string message) : 
    base(message) { }
    public ExistsEmployeeException(string message, Exception innerException) : 
    base(message, innerException) { }
}