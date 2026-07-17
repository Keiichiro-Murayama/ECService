using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;

using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// ProductFactoryの単体テスト
/// </summary>
[TestClass]
public class ProductFactoryTests
{
    [TestMethod(DisplayName = "UT-FAC-001 Entity一式からProductドメインへ復元できる")]
    public async Task Factory_ShouldRestoreProductDomain()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);
        var stockEntity = CreateProductStockEntity(productEntity);

        // Act
        var product = await factory.Factory(
            productEntity,
            stockEntity,
            categoryEntity);

        // Assert
        Assert.AreEqual(
            "55555555-5555-5555-5555-555555555555",
            product.ProductUuid);

        Assert.AreEqual("ボールペン", product.Name);
        Assert.AreEqual(120, product.Price);
        Assert.AreEqual("/images/pen.png", product.ImageUrl);
        Assert.AreEqual(0, product.DeleteFlg);

        Assert.AreEqual(
            "11111111-1111-1111-1111-111111111111",
            product.ProductCategory.CategoryUuid);

        Assert.AreEqual("文房具", product.ProductCategory.Name);

        Assert.AreEqual(
            "33333333-3333-3333-3333-333333333333",
            product.ProductStock.StockUuid);

        Assert.AreEqual(100, product.ProductStock.Quantity);
    }

    [TestMethod(DisplayName = "UT-FAC-002 削除フラグをProductドメインへ復元できる")]
    public async Task Factory_ShouldRestoreDeleteFlag()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);
        productEntity.DeleteFlag = 1;

        var stockEntity = CreateProductStockEntity(productEntity);

        // Act
        var product = await factory.Factory(
            productEntity,
            stockEntity,
            categoryEntity);

        // Assert
        Assert.AreEqual(1, product.DeleteFlg);
        Assert.IsTrue(product.IsDelete());
    }

    [TestMethod(DisplayName = "UT-FAC-003 ProductEntityがnullの場合はInternalExceptionをスローする")]
    public async Task Factory_ShouldThrowInternalException_WhenProductEntityIsNull()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);
        var stockEntity = CreateProductStockEntity(productEntity);

        // Act & Assert
        try
        {
            await factory.Factory(
                null!,
                stockEntity,
                categoryEntity);

            Assert.Fail("InternalExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InternalException exception)
        {
            Assert.AreEqual("引数productEntityがnullです。", exception.Message);
        }
    }

    [TestMethod(DisplayName = "UT-FAC-004 ProductStockEntityがnullの場合はInternalExceptionをスローする")]
    public async Task Factory_ShouldThrowInternalException_WhenProductStockEntityIsNull()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);

        // Act & Assert
        try
        {
            await factory.Factory(
                productEntity,
                null!,
                categoryEntity);

            Assert.Fail("InternalExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InternalException exception)
        {
            Assert.AreEqual("引数productStockEntityがnullです。", exception.Message);
        }
    }

    [TestMethod(DisplayName = "UT-FAC-005 ProductCategoryEntityがnullの場合はInternalExceptionをスローする")]
    public async Task Factory_ShouldThrowInternalException_WhenProductCategoryEntityIsNull()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);
        var stockEntity = CreateProductStockEntity(productEntity);

        // Act & Assert
        try
        {
            await factory.Factory(
                productEntity,
                stockEntity,
                null!);

            Assert.Fail("InternalExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InternalException exception)
        {
            Assert.AreEqual("引数productCategoryEntityがnullです。", exception.Message);
        }
    }

    [TestMethod(DisplayName = "UT-FAC-006 ImageUrlがnullの場合はDomainExceptionをスローする")]
    public async Task Factory_ShouldThrowDomainException_WhenImageUrlIsNull()
    {
        // Arrange
        var factory = CreateFactory();

        var categoryEntity = CreateProductCategoryEntity();
        var productEntity = CreateProductEntity(categoryEntity);
        productEntity.ImageUrl = null;

        var stockEntity = CreateProductStockEntity(productEntity);

        // Act & Assert
        try
        {
            await factory.Factory(
                productEntity,
                stockEntity,
                categoryEntity);

            Assert.Fail("DomainExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (DomainException exception)
        {
            Assert.AreEqual("画像をアップロードしてください", exception.Message);
        }
    }

    private static ProductFactory CreateFactory()
    {
        return new ProductFactory(
            new ProductCategoryEntityAdapter(),
            new ProductStockEntityAdapter());
    }

    private static ProductCategoryEntity CreateProductCategoryEntity()
    {
        return new ProductCategoryEntity
        {
            Id = 1,
            CategoryUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "文房具"
        };
    }

    private static ProductEntity CreateProductEntity(
        ProductCategoryEntity categoryEntity)
    {
        return new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("55555555-5555-5555-5555-555555555555"),
            ProductCategoryId = categoryEntity.Id,
            ProductCategory = categoryEntity,
            Name = "ボールペン",
            Price = 120,
            ImageUrl = "/images/pen.png",
            DeleteFlag = 0
        };
    }

    private static ProductStockEntity CreateProductStockEntity(
        ProductEntity productEntity)
    {
        return new ProductStockEntity
        {
            Id = 1,
            StockUuid = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ProductId = productEntity.Id,
            Product = productEntity,
            Quantity = 100
        };
    }
}