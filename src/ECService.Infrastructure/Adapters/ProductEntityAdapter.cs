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
IConverter<Product, ProductEntity>, IRestorer<Product, ProductEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductをProductEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Product</param>
    /// <returns>EFCore:ProductEntity</returns>
    public ProductEntity Convert(Product product)
    {
        return new ProductEntity
        {
            ProductUuid = Guid.Parse(product.ProductUuid),
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            DeleteFlag = product.DeleteFlg
            // ProductCategoryId は Repository で設定する
        };
    }

    /// <summary>
    /// ProductEntityからドメインオブジェクト:Productを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductEntity</param>
    /// <returns>ドメインオブジェクト:Product</returns>
    public Task<Product> RestoreAsync(ProductEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductEntityからドメインオブジェクト:Productを復元する
        var domain = new Product(target.ProductUuid, target.Name, target.Price, target.ImageUrl, target.DeleteFlag, ProductCategory., ProductStock.productStock);
        return Task.FromResult(domain);
    }

}