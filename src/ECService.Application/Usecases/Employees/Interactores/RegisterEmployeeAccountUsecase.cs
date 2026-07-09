using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Application.UseCases.UnitOfWorks;
using ECService.Application.Security;
using ECService.Application.Usecases.Employees.Interfaces;
namespace ECService.Application.Employees.Interactors;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するインターフェイスの実装
/// </summary>
public class RegisterEmployeeAccountUsecase : IRegisterEmployeeAccountUsecase
{
    private readonly IEmployeeAccountRepository _repository;
    private readonly IPasswordService _service;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">ユーザーCRUD操作リポジトリ</param>
    /// <param name="service">パスワードをハッシュ化するサービス</param>
    /// <param name="unitOfWork">トランザクション制御機能</param>
    public RegisterEmployeeAccountUsecase(
        IEmployeeAccountRepository repository,IPasswordService service,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _service    = service;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// ユーザーを登録する
    /// </summary>
    /// <param name="user">登録対象ユーザー</param>
    /// <returns></returns>
   public Task ExecuteAsync(EmployeeAccount employeeAccount)
    {
        // トランザクションを開始する
        await _unitOfWork.BeginAsync();
        try
        {
            // パスワードをハッシュ化する
            var passwordHash = _service.Hash(user.Password);
            // ハッシュ化したパスワードを設定する
            user.ChangePassword(passwordHash);
            // ユーザーを永続化する
            await _repository.CreateAsync(user);
            // トランザクションをコミットする
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            // トランザクションをロールバックする
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}