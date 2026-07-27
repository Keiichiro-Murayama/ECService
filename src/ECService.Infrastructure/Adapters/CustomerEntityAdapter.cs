using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Entities;

namespace ECService.Infrastructure.Adapters;

/// <summary>
/// 顧客Entityからドメインモデルへ復元するAdapter
/// </summary>
public class CustomerEntityAdapter
    : IRestorer<Customer, CustomerEntity>
{
    /// <summary>
    /// 顧客Entityから顧客ドメインモデルへ復元する
    /// </summary>
    /// <param name="target">顧客Entity</param>
    /// <returns>顧客ドメインモデル</returns>
    public Task<Customer> RestoreAsync(CustomerEntity target)
    {
        var customer = new Customer(
            target.CustomerUuid.ToString(),
            target.Username);

        return Task.FromResult(customer);
    }
}