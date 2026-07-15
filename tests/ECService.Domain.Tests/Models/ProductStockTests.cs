using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Domain.Tests.Models;

/// <summary>
/// ProductStockドメインオブジェクトの単体テスト
/// </summary>
[TestClass]
public class ProductStockTests
{
    /// <summary>
    /// DomainExceptionのメッセージとパラメータ名を確認する
    /// </summary>
    private static void AssertDomainException(
        Action action,
        string expectedMessage,
        string expectedParamName)
    {
        var exception =
            Assert.ThrowsExactly<DomainException>(action);

        Assert.AreEqual(
            expectedMessage,
            exception.Message);

        Assert.AreEqual(
            expectedParamName,
            exception.ParamName);
    }

    /// <summary>
    /// テスト用の商品在庫を生成する
    /// </summary>
    private static ProductStock CreateProductStock()
    {
        return ProductStock.Restore(
            "11111111-1111-1111-1111-111111111111",
            100);
    }

    // =========================================================
    // Create 正常系
    // =========================================================

    [TestMethod]
    public void Create_正常な在庫数を渡した場合_商品在庫が生成される()
    {
        // Arrange
        const int quantity = 100;

        // Act
        var productStock =
            ProductStock.Create(quantity);

        // Assert
        Assert.IsFalse(
            string.IsNullOrWhiteSpace(productStock.StockUuid));

        Assert.IsTrue(
            Guid.TryParse(productStock.StockUuid, out _));

        Assert.AreEqual(
            quantity,
            productStock.Quantity);
    }

    [TestMethod]
    public void Create_複数回実行した場合_異なるUUIDが生成される()
    {
        // Act
        var first =
            ProductStock.Create(100);

        var second =
            ProductStock.Create(100);

        // Assert
        Assert.AreNotEqual(
            first.StockUuid,
            second.StockUuid);
    }

    // =========================================================
    // Create 境界値
    // =========================================================

    [TestMethod]
    [DataRow(0)]
    [DataRow(1000)]
    public void Create_在庫数が境界値の場合_商品在庫が生成される(
        int quantity)
    {
        // Act
        var productStock =
            ProductStock.Create(quantity);

        // Assert
        Assert.AreEqual(
            quantity,
            productStock.Quantity);
    }

    // =========================================================
    // Create 異常系
    // =========================================================

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1001)]
    public void Create_在庫数が範囲外の場合_DomainExceptionが発生する(
        int quantity)
    {
        // Act & Assert
        AssertDomainException(
            () => ProductStock.Create(quantity),
            "在庫数は1000個以下で入力してください",
            "quantity");
    }

    // =========================================================
    // Restore
    // =========================================================

    [TestMethod]
    public void Restore_正常な値を渡した場合_指定された状態で復元される()
    {
        // Arrange
        const string stockUuid =
            "22222222-2222-2222-2222-222222222222";

        const int quantity = 250;

        // Act
        var productStock =
            ProductStock.Restore(
                stockUuid,
                quantity);

        // Assert
        Assert.AreEqual(
            stockUuid,
            productStock.StockUuid);

        Assert.AreEqual(
            quantity,
            productStock.Quantity);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1000)]
    public void Restore_在庫数が境界値の場合_指定された状態で復元される(
        int quantity)
    {
        // Arrange
        const string stockUuid =
            "33333333-3333-3333-3333-333333333333";

        // Act
        var productStock =
            ProductStock.Restore(
                stockUuid,
                quantity);

        // Assert
        Assert.AreEqual(
            stockUuid,
            productStock.StockUuid);

        Assert.AreEqual(
            quantity,
            productStock.Quantity);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1001)]
    public void Restore_在庫数が範囲外の場合_DomainExceptionが発生する(
        int quantity)
    {
        // Act & Assert
        AssertDomainException(
            () => ProductStock.Restore(
                "44444444-4444-4444-4444-444444444444",
                quantity),
            "在庫数は1000個以下で入力してください",
            "quantity");
    }

    // =========================================================
    // ValidateQuantity 正常系
    // =========================================================

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(500)]
    [DataRow(1000)]
    public void ValidateQuantity_在庫数が範囲内の場合_例外が発生しない(
        int quantity)
    {
        // Act
        ProductStock.ValidateQuantity(quantity);

        // Assert
        Assert.IsTrue(true);
    }

    // =========================================================
    // ValidateQuantity 異常系
    // =========================================================

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1001)]
    public void ValidateQuantity_在庫数が範囲外の場合_DomainExceptionが発生する(
        int quantity)
    {
        // Act & Assert
        AssertDomainException(
            () => ProductStock.ValidateQuantity(quantity),
            "在庫数は1000個以下で入力してください",
            "quantity");
    }

    // =========================================================
    // ChangeQuantity 正常系
    // =========================================================

    [TestMethod]
    public void ChangeQuantity_正常な在庫数を渡した場合_在庫数が変更される()
    {
        // Arrange
        var productStock =
            CreateProductStock();

        // Act
        productStock.ChangeQuantity(500);

        // Assert
        Assert.AreEqual(
            500,
            productStock.Quantity);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1000)]
    public void ChangeQuantity_在庫数が境界値の場合_在庫数が変更される(
        int quantity)
    {
        // Arrange
        var productStock =
            CreateProductStock();

        // Act
        productStock.ChangeQuantity(quantity);

        // Assert
        Assert.AreEqual(
            quantity,
            productStock.Quantity);
    }

    // =========================================================
    // ChangeQuantity 異常系
    // =========================================================

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1001)]
    public void ChangeQuantity_在庫数が範囲外の場合_例外が発生して元の値が保持される(
        int quantity)
    {
        // Arrange
        var productStock =
            CreateProductStock();

        var originalQuantity =
            productStock.Quantity;

        // Act & Assert
        AssertDomainException(
            () => productStock.ChangeQuantity(quantity),
            "在庫数は1000個以下で入力してください",
            "quantity");

        Assert.AreEqual(
            originalQuantity,
            productStock.Quantity);
    }

    // =========================================================
    // Entityの同一性判定
    // =========================================================

    [TestMethod]
    public void Equals_StockUuidが同じ場合_同一の商品在庫と判定される()
    {
        // Arrange
        const string stockUuid =
            "55555555-5555-5555-5555-555555555555";

        var first =
            ProductStock.Restore(
                stockUuid,
                100);

        var second =
            ProductStock.Restore(
                stockUuid,
                500);

        // Act
        var result =
            first.Equals(second);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(first == second);

        Assert.AreEqual(
            first.GetHashCode(),
            second.GetHashCode());
    }

    [TestMethod]
    public void Equals_StockUuidが異なる場合_別の商品在庫と判定される()
    {
        // Arrange
        var first =
            ProductStock.Restore(
                "66666666-6666-6666-6666-666666666666",
                100);

        var second =
            ProductStock.Restore(
                "77777777-7777-7777-7777-777777777777",
                100);

        // Act
        var result =
            first.Equals(second);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(first != second);
    }
}