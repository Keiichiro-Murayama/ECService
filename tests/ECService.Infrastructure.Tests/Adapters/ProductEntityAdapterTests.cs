using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// ProductEntityAdapterの単体テスト
/// </summary>
[TestClass]
public class ProductEntityAdapterTests
{
    [TestMethod(DisplayName = "UT-ADP-014 ProductドメインをProductEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertProductDomainToEntity()
    {
        // Arrange
        var adapter = new ProductEntityAdapter();

        var product = CreateProduct();

        // Act
        var entity = await adapter.ConvertAsync(product);

        // Assert
        Assert.AreEqual(
            Guid.Parse("55555555-5555-5555-5555-555555555555"),
            entity.ProductUuid);

        Assert.AreEqual("ボールペン", entity.Name);
        Assert.AreEqual(120, entity.Price);
        Assert.AreEqual("/images/pen.png", entity.ImageUrl);
        Assert.AreEqual(0, entity.DeleteFlag);
    }

    [TestMethod(DisplayName = "UT-ADP-015 削除フラグをEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertDeleteFlag()
    {
        // Arrange
        var adapter = new ProductEntityAdapter();

        var product = CreateProduct();
        product.Delete();

        // Act
        var entity = await adapter.ConvertAsync(product);

        // Assert
        Assert.AreEqual(1, entity.DeleteFlag);
    }

    [TestMethod(DisplayName = "UT-ADP-016 domainがnullの場合はInternalExceptionをスローする")]
    public async Task ConvertAsync_ShouldThrowInternalException_WhenDomainIsNull()
    {
        // Arrange
        var adapter = new ProductEntityAdapter();

        // Act & Assert
        try
        {
            await adapter.ConvertAsync(null!);

            Assert.Fail("InternalExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InternalException exception)
        {
            Assert.AreEqual("引数domainがnullです。", exception.Message);
        }
    }

    private static Product CreateProduct()
    {
        var category = new ProductCategory(
            "11111111-1111-1111-1111-111111111111",
            "文房具");

        var stock = ProductStock.Restore(
            "33333333-3333-3333-3333-333333333333",
            100);

        return Product.Restore(
            "55555555-5555-5555-5555-555555555555",
            "ボールペン",
            120,
            "/images/pen.png",
            category,
            0,
            stock);
    }
}