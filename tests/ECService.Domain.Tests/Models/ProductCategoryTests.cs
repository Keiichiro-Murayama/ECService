using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Domain.Tests.Models;

/// <summary>
/// ProductCategoryドメインオブジェクトの単体テスト
/// </summary>
[TestClass]
public class ProductCategoryTests
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

    // =========================================================
    // Create 正常系
    // =========================================================

    [TestMethod]
    public void Create_正常なカテゴリ名を渡した場合_商品カテゴリが生成される()
    {
        // Arrange
        const string name = "文房具";

        // Act
        var category =
            ProductCategory.Create(name);

        // Assert
        Assert.IsFalse(
            string.IsNullOrWhiteSpace(category.CategoryUuid));

        Assert.IsTrue(
            Guid.TryParse(category.CategoryUuid, out _));

        Assert.AreEqual(
            name,
            category.Name);
    }

    [TestMethod]
    public void Create_複数回実行した場合_異なるUUIDが生成される()
    {
        // Act
        var first =
            ProductCategory.Create("文房具");

        var second =
            ProductCategory.Create("雑貨");

        // Assert
        Assert.AreNotEqual(
            first.CategoryUuid,
            second.CategoryUuid);
    }

    // =========================================================
    // カテゴリ名の未入力
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_カテゴリ名が未入力の場合_DomainExceptionが発生する(
        string? name)
    {
        // Act & Assert
        AssertDomainException(
            () => ProductCategory.Create(name!),
            "カテゴリ名を入力してください。",
            "name");
    }

    // =========================================================
    // カテゴリ名の境界値
    // =========================================================

    [TestMethod]
    public void Create_カテゴリ名が1文字の場合_商品カテゴリが生成される()
    {
        // Arrange
        const string name = "文";

        // Act
        var category =
            ProductCategory.Create(name);

        // Assert
        Assert.AreEqual(
            name,
            category.Name);

        Assert.AreEqual(
            1,
            category.Name.Length);
    }

    [TestMethod]
    public void Create_カテゴリ名が30文字の場合_商品カテゴリが生成される()
    {
        // Arrange
        var name =
            new string('あ', 30);

        // Act
        var category =
            ProductCategory.Create(name);

        // Assert
        Assert.AreEqual(
            name,
            category.Name);

        Assert.AreEqual(
            30,
            category.Name.Length);
    }

    [TestMethod]
    public void Create_カテゴリ名が31文字の場合_DomainExceptionが発生する()
    {
        // Arrange
        var name =
            new string('あ', 31);

        // Act & Assert
        AssertDomainException(
            () => ProductCategory.Create(name),
            "カテゴリ名は30文字以内で入力してください。",
            "name");
    }

    // =========================================================
    // ValidateName 正常系
    // =========================================================

    [TestMethod]
    public void ValidateName_正常なカテゴリ名を渡した場合_例外が発生しない()
    {
        // Arrange
        const string name = "パソコン周辺機器";

        // Act
        ProductCategory.ValidateName(name);

        // Assert
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void ValidateName_30文字を渡した場合_例外が発生しない()
    {
        // Arrange
        var name =
            new string('あ', 30);

        // Act
        ProductCategory.ValidateName(name);

        // Assert
        Assert.IsTrue(true);
    }

    // =========================================================
    // ValidateName 異常系
    // =========================================================

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void ValidateName_カテゴリ名が未入力の場合_DomainExceptionが発生する(
        string? name)
    {
        // Act & Assert
        AssertDomainException(
            () => ProductCategory.ValidateName(name!),
            "カテゴリ名を入力してください。",
            "name");
    }

    [TestMethod]
    public void ValidateName_31文字を渡した場合_DomainExceptionが発生する()
    {
        // Arrange
        var name =
            new string('あ', 31);

        // Act & Assert
        AssertDomainException(
            () => ProductCategory.ValidateName(name),
            "カテゴリ名は30文字以内で入力してください。",
            "name");
    }

    // =========================================================
    // Entityの同一性判定
    // =========================================================

    [TestMethod]
    public void Equals_CategoryUuidが同じ場合_同一の商品カテゴリと判定される()
    {
        // Arrange
        const string categoryUuid =
            "11111111-1111-1111-1111-111111111111";

        var first =
            new ProductCategory(
                categoryUuid,
                "文房具");

        var second =
            new ProductCategory(
                categoryUuid,
                "雑貨");

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
    public void Equals_CategoryUuidが異なる場合_別の商品カテゴリと判定される()
    {
        // Arrange
        var first =
            new ProductCategory(
                "22222222-2222-2222-2222-222222222222",
                "文房具");

        var second =
            new ProductCategory(
                "33333333-3333-3333-3333-333333333333",
                "文房具");

        // Act
        var result =
            first.Equals(second);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(first != second);
    }
}