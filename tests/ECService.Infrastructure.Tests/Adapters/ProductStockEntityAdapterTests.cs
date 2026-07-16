using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// ProductStockEntityAdapterの単体テスト
/// </summary>
[TestClass]
public class ProductStockEntityAdapterTests
{
    [TestMethod(DisplayName = "UT-ADP-010 ProductStockドメインをProductStockEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertProductStockDomainToEntity()
    {
        // Arrange
        var adapter = new ProductStockEntityAdapter();

        var stock = ProductStock.Restore(
            "33333333-3333-3333-3333-333333333333",
            100);

        // Act
        var entity = await adapter.ConvertAsync(stock);

        // Assert
        Assert.AreEqual(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            entity.StockUuid);

        Assert.AreEqual(100, entity.Quantity);
    }

    [TestMethod(DisplayName = "UT-ADP-011 ProductStockEntityをProductStockドメインへ復元できる")]
    public async Task RestoreAsync_ShouldRestoreProductStockEntityToDomain()
    {
        // Arrange
        var adapter = new ProductStockEntityAdapter();

        var entity = new ProductStockEntity
        {
            Id = 1,
            StockUuid = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            ProductId = 1,
            Quantity = 100
        };

        // Act
        var stock = await adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(
            "33333333-3333-3333-3333-333333333333",
            stock.StockUuid);

        Assert.AreEqual(100, stock.Quantity);
    }

    [TestMethod(DisplayName = "UT-ADP-012 在庫数0をEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertZeroQuantity()
    {
        // Arrange
        var adapter = new ProductStockEntityAdapter();

        var stock = ProductStock.Restore(
            "33333333-3333-3333-3333-333333333333",
            0);

        // Act
        var entity = await adapter.ConvertAsync(stock);

        // Assert
        Assert.AreEqual(0, entity.Quantity);
    }

    [TestMethod(DisplayName = "UT-ADP-013 targetがnullの場合はInternalExceptionをスローする")]
    public async Task RestoreAsync_ShouldThrowInternalException_WhenTargetIsNull()
    {
        // Arrange
        var adapter = new ProductStockEntityAdapter();

        // Act & Assert
        try
        {
            await adapter.RestoreAsync(null!);

            Assert.Fail("InternalExceptionが発生する想定だったが、発生しなかった。");
        }
        catch (InternalException exception)
        {
            Assert.AreEqual("引数targetがnullです。", exception.Message);
        }
    }
}