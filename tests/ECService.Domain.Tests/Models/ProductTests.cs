using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Domain.Tests.Models;

/// <summary>
/// Productドメインオブジェクトの単体テスト
/// </summary>
[TestClass]
public class ProductTests
{
    /// <summary>
    /// テスト用の商品カテゴリを生成する
    /// </summary>
    private static ProductCategory CreateCategory(
        string categoryUuid = "11111111-1111-1111-1111-111111111111",
        string name = "文房具")
    {
        return new ProductCategory(categoryUuid, name);
    }

    /// <summary>
    /// テスト用の商品在庫を生成する
    /// </summary>
    private static ProductStock CreateStock(
        string stockUuid = "22222222-2222-2222-2222-222222222222",
        int quantity = 10)
    {
        return ProductStock.Restore(stockUuid, quantity);
    }

    /// <summary>
    /// テスト用の商品を生成する
    /// </summary>
    private static Product CreateProduct()
    {
        return Product.Restore(
            "33333333-3333-3333-3333-333333333333",
            "ボールペン",
            120,
            "/images/ballpen.png",
            CreateCategory(),
            0,
            CreateStock());
    }

    /// <summary>
    /// DomainExceptionの内容を確認する
    /// </summary>
    private static void AssertDomainException(
        Action action,
        string expectedMessage,
        string expectedParamName)
    {
        var exception = Assert.ThrowsExactly<DomainException>(action);

        Assert.AreEqual(expectedMessage, exception.Message);
        Assert.AreEqual(expectedParamName, exception.ParamName);
    }

    // =========================================================
    // Create
    // =========================================================

    [TestMethod]
    public void Create_正常な値を渡した場合_商品が生成される()
    {
        // Arrange
        var category = CreateCategory();
        var stock = CreateStock();

        // Act
        var product = Product.Create(
            "ボールペン",
            120,
            "/images/ballpen.png",
            category,
            stock);

        // Assert
        Assert.IsFalse(string.IsNullOrWhiteSpace(product.ProductUuid));
        Assert.IsTrue(Guid.TryParse(product.ProductUuid, out _));

        Assert.AreEqual("ボールペン", product.Name);
        Assert.AreEqual(120, product.Price);
        Assert.AreEqual("/images/ballpen.png", product.ImageUrl);
        Assert.AreSame(category, product.ProductCategory);
        Assert.AreEqual(0, product.DeleteFlg);
        Assert.AreSame(stock, product.ProductStock);
        Assert.IsFalse(product.IsDelete());
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_商品名が未入力の場合_DomainExceptionが発生する(
        string? name)
    {
        // Arrange
        var category = CreateCategory();
        var stock = CreateStock();

        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                name!,
                120,
                "/images/ballpen.png",
                category,
                stock),
            "商品名を入力してください",
            "name");
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(21)]
    public void Create_商品名が文字数範囲外の場合_DomainExceptionが発生する(
        int length)
    {
        // Arrange
        var name = new string('あ', length);
        var category = CreateCategory();
        var stock = CreateStock();

        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                name,
                120,
                "/images/product.png",
                category,
                stock),
            "商品名は2～20文字で入力してください",
            "name");
    }

    [TestMethod]
    [DataRow(2)]
    [DataRow(20)]
    public void Create_商品名が境界値の場合_商品が生成される(
        int length)
    {
        // Arrange
        var name = new string('あ', length);

        // Act
        var product = Product.Create(
            name,
            120,
            "/images/product.png",
            CreateCategory(),
            CreateStock());

        // Assert
        Assert.AreEqual(name, product.Name);
    }

    [TestMethod]
    [DataRow(-1, "価格は0円以上で入力してください")]
    [DataRow(1000001, "価格は100万円以下で入力してください")]
    public void Create_価格が範囲外の場合_DomainExceptionが発生する(
        int price,
        string expectedMessage)
    {
        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                "ボールペン",
                price,
                "/images/ballpen.png",
                CreateCategory(),
                CreateStock()),
            expectedMessage,
            "price");
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1000000)]
    public void Create_価格が境界値の場合_商品が生成される(
        int price)
    {
        // Act
        var product = Product.Create(
            "ボールペン",
            price,
            "/images/ballpen.png",
            CreateCategory(),
            CreateStock());

        // Assert
        Assert.AreEqual(price, product.Price);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Create_画像URLが未入力の場合_DomainExceptionが発生する(
        string? imageUrl)
    {
        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                "ボールペン",
                120,
                imageUrl!,
                CreateCategory(),
                CreateStock()),
            "画像をアップロードしてください",
            "imageUrl");
    }

    [TestMethod]
    public void Create_商品カテゴリがnullの場合_DomainExceptionが発生する()
    {
        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                "ボールペン",
                120,
                "/images/ballpen.png",
                null!,
                CreateStock()),
            "カテゴリを選択してください",
            "productCategory");
    }

    [TestMethod]
    public void Create_商品在庫がnullの場合_DomainExceptionが発生する()
    {
        // Act & Assert
        AssertDomainException(
            () => Product.Create(
                "ボールペン",
                120,
                "/images/ballpen.png",
                CreateCategory(),
                null!),
            "在庫数を入力してください",
            "productStock");
    }

    // =========================================================
    // Restore
    // =========================================================

    [TestMethod]
    public void Restore_正常な値を渡した場合_指定された状態で復元される()
    {
        // Arrange
        const string productUuid =
            "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

        var category = CreateCategory();
        var stock = CreateStock();

        // Act
        var product = Product.Restore(
            productUuid,
            "シャープペンシル",
            500,
            "/images/sharppen.png",
            category,
            1,
            stock);

        // Assert
        Assert.AreEqual(productUuid, product.ProductUuid);
        Assert.AreEqual("シャープペンシル", product.Name);
        Assert.AreEqual(500, product.Price);
        Assert.AreEqual("/images/sharppen.png", product.ImageUrl);
        Assert.AreSame(category, product.ProductCategory);
        Assert.AreEqual(1, product.DeleteFlg);
        Assert.AreSame(stock, product.ProductStock);
        Assert.IsTrue(product.IsDelete());
    }

    // =========================================================
    // ChangeName
    // =========================================================

    [TestMethod]
    public void ChangeName_正常な商品名を渡した場合_商品名が変更される()
    {
        // Arrange
        var product = CreateProduct();

        // Act
        product.ChangeName("油性ボールペン");

        // Assert
        Assert.AreEqual("油性ボールペン", product.Name);
    }

    [TestMethod]
    public void ChangeName_不正な商品名を渡した場合_例外が発生して元の名前が保持される()
    {
        // Arrange
        var product = CreateProduct();
        var originalName = product.Name;

        // Act & Assert
        AssertDomainException(
            () => product.ChangeName("あ"),
            "商品名は2～20文字で入力してください",
            "name");

        Assert.AreEqual(originalName, product.Name);
    }

    // =========================================================
    // ChangePrice
    // =========================================================

    [TestMethod]
    public void ChangePrice_正常な価格を渡した場合_価格が変更される()
    {
        // Arrange
        var product = CreateProduct();

        // Act
        product.ChangePrice(300);

        // Assert
        Assert.AreEqual(300, product.Price);
    }

    [TestMethod]
    public void ChangePrice_不正な価格を渡した場合_例外が発生して元の価格が保持される()
    {
        // Arrange
        var product = CreateProduct();
        var originalPrice = product.Price;

        // Act & Assert
        AssertDomainException(
            () => product.ChangePrice(-1),
            "価格は0円以上で入力してください",
            "price");

        Assert.AreEqual(originalPrice, product.Price);
    }

    // =========================================================
    // ChangeImageUrl
    // =========================================================

    [TestMethod]
    public void ChangeImageUrl_正常な画像URLを渡した場合_画像URLが変更される()
    {
        // Arrange
        var product = CreateProduct();

        // Act
        product.ChangeImageUrl("/images/new-ballpen.png");

        // Assert
        Assert.AreEqual(
            "/images/new-ballpen.png",
            product.ImageUrl);
    }

    [TestMethod]
    public void ChangeImageUrl_未入力の場合_例外が発生して元の画像URLが保持される()
    {
        // Arrange
        var product = CreateProduct();
        var originalImageUrl = product.ImageUrl;

        // Act & Assert
        AssertDomainException(
            () => product.ChangeImageUrl(" "),
            "画像をアップロードしてください",
            "imageUrl");

        Assert.AreEqual(originalImageUrl, product.ImageUrl);
    }

    // =========================================================
    // ChangeCategory
    // =========================================================

    [TestMethod]
    public void ChangeCategory_正常なカテゴリを渡した場合_カテゴリが変更される()
    {
        // Arrange
        var product = CreateProduct();

        var newCategory = CreateCategory(
            "44444444-4444-4444-4444-444444444444",
            "パソコン周辺機器");

        // Act
        product.ChangeCategory(newCategory);

        // Assert
        Assert.AreSame(newCategory, product.ProductCategory);
    }

    [TestMethod]
    public void ChangeCategory_nullを渡した場合_例外が発生して元のカテゴリが保持される()
    {
        // Arrange
        var product = CreateProduct();
        var originalCategory = product.ProductCategory;

        // Act & Assert
        AssertDomainException(
            () => product.ChangeCategory(null!),
            "カテゴリを選択してください",
            "productCategory");

        Assert.AreSame(
            originalCategory,
            product.ProductCategory);
    }

    // =========================================================
    // ChangeStock
    // =========================================================

    [TestMethod]
    public void ChangeStock_正常な在庫を渡した場合_商品在庫が変更される()
    {
        // Arrange
        var product = CreateProduct();

        var newStock = CreateStock(
            "55555555-5555-5555-5555-555555555555",
            50);

        // Act
        product.ChangeStock(newStock);

        // Assert
        Assert.AreSame(newStock, product.ProductStock);
    }

    [TestMethod]
    public void ChangeStock_nullを渡した場合_例外が発生して元の商品在庫が保持される()
    {
        // Arrange
        var product = CreateProduct();
        var originalStock = product.ProductStock;

        // Act & Assert
        AssertDomainException(
            () => product.ChangeStock(null!),
            "在庫数を入力してください",
            "productStock");

        Assert.AreSame(originalStock, product.ProductStock);
    }

    // =========================================================
    // Delete・IsDelete
    // =========================================================

    [TestMethod]
    public void Delete_未削除の商品に実行した場合_削除済みになる()
    {
        // Arrange
        var product = CreateProduct();

        Assert.AreEqual(0, product.DeleteFlg);
        Assert.IsFalse(product.IsDelete());

        // Act
        product.Delete();

        // Assert
        Assert.AreEqual(1, product.DeleteFlg);
        Assert.IsTrue(product.IsDelete());
    }

    [TestMethod]
    [DataRow(0, false)]
    [DataRow(1, true)]
    public void IsDelete_削除フラグに応じた結果を返す(
        int deleteFlg,
        bool expected)
    {
        // Arrange
        var product = Product.Restore(
            "66666666-6666-6666-6666-666666666666",
            "ボールペン",
            120,
            "/images/ballpen.png",
            CreateCategory(),
            deleteFlg,
            CreateStock());

        // Act
        var result = product.IsDelete();

        // Assert
        Assert.AreEqual(expected, result);
    }

    // =========================================================
    // Entityの同一性
    // =========================================================

    [TestMethod]
    public void Equals_ProductUuidが同じ場合_同一の商品と判定される()
    {
        // Arrange
        const string productUuid =
            "77777777-7777-7777-7777-777777777777";

        var first = Product.Restore(
            productUuid,
            "ボールペン",
            120,
            "/images/ballpen.png",
            CreateCategory(),
            0,
            CreateStock());

        var second = Product.Restore(
            productUuid,
            "異なる商品名",
            500,
            "/images/other.png",
            CreateCategory(),
            1,
            CreateStock());

        // Act & Assert
        Assert.IsTrue(first.Equals(second));
        Assert.IsTrue(first == second);
        Assert.AreEqual(
            first.GetHashCode(),
            second.GetHashCode());
    }

    [TestMethod]
    public void Equals_ProductUuidが異なる場合_別の商品と判定される()
    {
        // Arrange
        var first = Product.Restore(
            "88888888-8888-8888-8888-888888888888",
            "ボールペン",
            120,
            "/images/ballpen.png",
            CreateCategory(),
            0,
            CreateStock());

        var second = Product.Restore(
            "99999999-9999-9999-9999-999999999999",
            "ボールペン",
            120,
            "/images/ballpen.png",
            CreateCategory(),
            0,
            CreateStock());

        // Act & Assert
        Assert.IsFalse(first.Equals(second));
        Assert.IsTrue(first != second);
    }
}