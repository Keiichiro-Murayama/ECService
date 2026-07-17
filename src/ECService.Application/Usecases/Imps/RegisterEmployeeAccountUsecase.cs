using ECService.Application.Authentications;
using ECService.Application.Exceptions;
using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

using DomainException = ECService.Domain.Exceptions.DomainException; //石原:追加

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 担当者アカウント登録ユースケースの実装
/// </summary>
public class RegisterEmployeeAccountUsecase : IRegisterEmployeeAccountUsecase
{
    private readonly IEmployeeAccountRepository _employeeAccountRepository;
    private readonly IPasswordService _passwordService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterEmployeeAccountUsecase(
        IEmployeeAccountRepository employeeAccountRepository,
        IPasswordService passwordService,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork)
    {
        _employeeAccountRepository = employeeAccountRepository;
        _passwordService = passwordService;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// 担当者アカウントを登録する
    /// </summary>
    /// <param name="employeeUuid">社員UUID</param>
    /// <param name="accountName">アカウント名</param>
    /// <param name="password">パスワード</param>
    public async Task ExecuteAsync(
        string employeeUuid,
        string accountName,
        string password)
    {
        //石原:追加 未入力・空白チェック
        if (string.IsNullOrWhiteSpace(employeeUuid) ||
            string.IsNullOrWhiteSpace(accountName) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException(
                "入力値に不備があります。",
                nameof(employeeUuid));
        }

        var trimmedEmployeeUuid = employeeUuid.Trim(); //石原:追加
        var trimmedAccountName = accountName.Trim(); //石原:追加

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // ① アカウント名が既に登録されているか確認する
            var existsAccountName = await _employeeAccountRepository
                .ExistsByAccountNameAsync(trimmedAccountName);

            if (existsAccountName)
            {
                throw new ExistsAccountException(
                    "このアカウント名は既に登録されています。");
            }

            //石原:追加 同じ社員UUIDで既に担当者アカウントが登録されているか確認する
            var existsEmployeeAccount = await _employeeAccountRepository
                .ExistsByEmployeeUuidAsync(trimmedEmployeeUuid);

            if (existsEmployeeAccount)
            {
                throw new ExistsAccountException(
                    "この社員には既に担当者アカウントが登録されています。");
            }

            // ② 選択された社員がDBに存在するか確認する
            var employee = await _employeeRepository
                .SelectByUuidAsync(trimmedEmployeeUuid);

            if (employee is null)
            {
                throw new ExistsEmployeeException(
                    "指定された社員IDが存在しません。");
            }

            // ③ 平文パスワードをハッシュ化する
            var passwordHash = _passwordService.Hash(password);

            // ④ ハッシュ化済みパスワードでアカウントを生成する
            var employeeAccount = EmployeeAccount.Create(
                trimmedAccountName,
                password,
                passwordHash,
                employee);

            // ⑤ 担当者アカウントを永続化する
            await _employeeAccountRepository.CreateAsync(employeeAccount);

            // ⑥ トランザクションを確定する
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw; //石原:追加 Controllerに例外を返すために再スローする
        }
    }
}