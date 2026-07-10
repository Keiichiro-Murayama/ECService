using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Application.UseCases.UnitOfWorks;
using ECService.Application.Security;
using ECService.Application.Usecases.Interfaces;
using ECService.Application.Exceptions;
namespace ECService.Application.Imps;
/// <summary>
/// ユースケース:[ユーザーを登録する]を実現するインターフェイスの実装
/// </summary>
public class RegisterEmployeeAccountUsecase : IRegisterEmployeeAccountUsecase
{
    private readonly IEmployeeAccountRepository _employeeAccountRepository;
    private readonly IPasswordService _service;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="repository">ユーザーCRUD操作リポジトリ</param>
    /// <param name="service">パスワードをハッシュ化するサービス</param>
    /// <param name="unitOfWork">トランザクション制御機能</param>
    public RegisterEmployeeAccountUsecase(
        IEmployeeAccountRepository employeeAccountRepository, IPasswordService service,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeAccountRepository = employeeAccountRepository;
        _service = service;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// ユーザーを登録する
    /// </summary>
    /// <param name="user">登録対象ユーザー</param>
    /// <returns></returns>
    public async Task ExecuteAsync(EmployeeAccount employeeAccount)
    {
        // トランザクションを開始する
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // パスワードをハッシュ化する
            var passwordHash = _service.Hash(employeeAccount.Password);
            // ハッシュ化したパスワードを設定する
            employeeAccount.ChangePassword(passwordHash);
            var exists = await _employeeAccountRepository
          .ExistsByAccountNameAsync(accountName);

            if (exists)
            {
                throw new ExistsAccountException(
                    "このアカウント名は既に使用されています。",
                    nameof(accountName));
            }
            var employee = await _employeeRepository
         .SelectByUuidAsync(employeeUuid);

            if (employee is null)
            {
                throw new ExistsEmployeeException(
                    "選択された社員が存在しません。",
                    nameof(employeeUuid));
            }
            // ユーザーを永続化する
            await _employeeAccountRepository.CreateAsync(employeeAccount);
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