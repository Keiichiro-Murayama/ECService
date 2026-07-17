using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECService.Infrastructure.Repositories;

/// <summary>
/// 担当者アカウント情報を操作するRepository。
/// </summary>
public class EmployeeAccountRepository : IEmployeeAccountRepository
{
    private readonly AppDbContext _context;
    private readonly EmployeeAccountEntityAdapter _adapter;

    public EmployeeAccountRepository(
        AppDbContext context,
        EmployeeAccountEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// アカウント名に一致する担当者アカウントを取得する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>担当者アカウント。存在しない場合はnull。</returns>
    public async Task<EmployeeAccount?> SelectByAccountNameAsync(string accountName)
    {
        var entity = await _context.EmployeeAccounts
            .Include(employeeAccount => employeeAccount.Employee)
            .ThenInclude(employee => employee.Department)
            .FirstOrDefaultAsync(employeeAccount =>
                employeeAccount.Name == accountName);

        if (entity is null)
        {
            return null;
        }

        return await _adapter.RestoreAsync(entity);
    }

    /// <summary>
    /// 指定したアカウント名が既に存在するか確認する。
    /// </summary>
    /// <param name="accountName">アカウント名。</param>
    /// <returns>存在する場合true。</returns>
    public async Task<bool> ExistsByAccountNameAsync(string accountName)
    {
        return await _context.EmployeeAccounts
            .AnyAsync(employeeAccount =>
                employeeAccount.Name == accountName);
    }

    /// <summary>
    /// 担当者アカウントを登録する。
    /// </summary>
    /// <param name="employeeAccount">担当者アカウント。</param>
    public async Task CreateAsync(EmployeeAccount employeeAccount)
    {
        var entity = await _adapter.ConvertAsync(employeeAccount);

        var employeeUuid = Guid.Parse(employeeAccount.Employee.EmployeeUuid);

        var employeeEntity = await _context.Employees
            .FirstOrDefaultAsync(employee =>
                employee.EmployeeUuid == employeeUuid);

        if (employeeEntity is null)
        {
            throw new InvalidOperationException("社員が見つかりません。");
        }

        entity.EmployeeId = employeeEntity.Id;

        await _context.EmployeeAccounts.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// アカウントロックアウトの状態を更新する。
    /// </summary>
    public async Task<EmployeeAccount?> FindByUsernameAsync(string name)
    {
        // 1. まずはDBから「Entity型」で取得する
        var entity = await _context.EmployeeAccounts
        .FirstOrDefaultAsync(a => a.Name == name);

         if (entity == null) return null;

         // 2. EntityからドメインのModelへ詰め替えて返す（プロジェクト共通の変換メソッドがあればそれを使う）
        return await _adapter.RestoreAsync(entity);
    }

    public async Task UpdateAsync(EmployeeAccount account)
    {
        // ドメインモデルをインフラ層のEntity型に逆変換する
        var entity = await _adapter.ConvertAsync(account); 

         // 変換後のEntityをUpdateに渡す
         _context.EmployeeAccounts.Update(entity);
         await _context.SaveChangesAsync();
    }

    public async Task RecordLoginFailureAsync(string name)
{
    // DBから直接インフラ層のEntityを引っ張る
    var entity = await _context.EmployeeAccounts
        .FirstOrDefaultAsync(a => a.Name == name);

    if (entity != null)
    {
        var now = DateTime.UtcNow;

        // 1. カウントを増やす
        entity.AccessFailedCount++;

        // 2. 5回に達したら10分ロック
        if (entity.AccessFailedCount >= 5)
        {
            entity.LockoutEnd = now.AddMinutes(10);
        }

        // 3. DBに保存
        await _context.SaveChangesAsync();
    }
}

public async Task ResetLoginFailureAsync(string name)
{
    var entity = await _context.EmployeeAccounts
        .FirstOrDefaultAsync(a => a.Name == name);

    if (entity != null)
    {
        entity.AccessFailedCount = 0;
        entity.LockoutEnd = null;
        await _context.SaveChangesAsync();
    }
}

//石原:追加 同じ社員UUIDで既に担当者アカウントが登録されているか確認する
public async Task<bool> ExistsByEmployeeUuidAsync(string employeeUuid)
{
    if (!Guid.TryParse(employeeUuid, out var parsedEmployeeUuid))
    {
        return false;
    }

    return await _context.EmployeeAccounts
        .AnyAsync(account => account.Employee.EmployeeUuid == parsedEmployeeUuid);
}
}
