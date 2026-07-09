using ECService.Domain.Adapters;
using ECService.Domain.Models;
using ECService.Infrastructure.Exceptions;
using ECService.Infrastructure.Entities;
namespace ECService.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductとProductEntityの相互変換クラス
/// </summary> 
/// <typeparam name="Product">ドメインオブジェクト:Product</typeparam>
/// <typeparam name="ProductEntity">EFCore:ProductEntity</typeparam>
public class ProductEntityAdapter :
IConverter<Product, ProductEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductをProductEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Product</param>
    /// <returns>EFCore:ProductEntity</returns>
    public Task<ProductEntity> ConvertAsync(Product domain)
    {

        _ = domain ?? throw new InternalException("引数domainがnullです。");

        var entity = new ProductEntity();
        entity.ProductUuid = Guid.Parse(domain.ProductUuid);
        entity.Name = domain.Name;
        entity.Price = domain.Price;
        entity.ImageUrl = domain.ImageUrl;
        entity.DeleteFlag = domain.DeleteFlg;
        return Task.FromResult(entity);

    }
}