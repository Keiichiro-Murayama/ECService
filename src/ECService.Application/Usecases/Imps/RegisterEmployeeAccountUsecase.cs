using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Application.Authentications;
using ECService.Application.Usecases.Interfaces;
using ECService.Application.Exceptions;

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
    /// <param name="employeeAccount">登録対象の担当者アカウント</param>
    public async Task ExecuteAsync(
        string employeeUuid,
        string accountName,
        string password)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // ① アカウント名が既に登録されているか確認する
            var exists = await _employeeAccountRepository
                .ExistsByAccountNameAsync(accountName);

            if (exists)
            {
                throw new ExistsAccountException(
                    "このアカウント名は既に使用されています。");
            }

            // ② 選択された社員がDBに存在するか確認する
            var employee = await _employeeRepository
                .SelectByUuidAsync(employeeUuid);

            if (employee is null)
            {
                throw new ExistsEmployeeException(
                    "選択された社員が存在しません。");
            }
            // 生パスワードを持つアカウントを生成
            var employeeAccount = EmployeeAccount.Create(
                accountName,
                password,
                employee);
            // ③ 平文パスワードをハッシュ化する
            var passwordHash = _passwordService.Hash(password);

            // ④ ハッシュ化済みパスワードをアカウントに設定する
            employeeAccount.ChangePassword(passwordHash);

            // ⑤ 担当者アカウントを永続化する
            await _employeeAccountRepository.CreateAsync(employeeAccount);

            // ⑥ トランザクションを確定する
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}