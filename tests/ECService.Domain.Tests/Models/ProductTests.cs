using System.Reflection;
using System.Runtime.Serialization;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using Xunit;
using Xunit.Abstractions;

namespace ECService.Domain.Tests.Models;

/// <summary>
/// Product ドメインモデルの単体テスト
///
/// 商品のバリデーションチェックおよび
/// 削除フラグなどの業務ルールを検証する。
/// </summary>
public class ProductTests
{
    private readonly ITestOutputHelper _output;

    public ProductTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private void Log(string message)
    {
        Console.WriteLine(message);
        _output.WriteLine(message);
    }

    [Fact(DisplayName = "正常な商品情報でProductを復元できる")]
    public void Product_Restore_ValidProduct_ReturnsProduct()
    {
        // Arrange
        Log("Product_Restore_ValidProduct_ReturnsProduct：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(10);

        var productUuid = "b7af7239-108b-4698-b2a7-2fe4469b275a";
        var name = "エコバッグ";
        var price = 880;
        var imageUrl = "https://example.com/images/bag.jpg";
        var deleteFlg = 0;

        // Act
        var result = Product.Restore(
            productUuid,
            name,
            price,
            imageUrl,
            productCategory,
            deleteFlg,
            productStock);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productUuid, result.ProductUuid);
        Assert.Equal(name, result.Name);
        Assert.Equal(price, result.Price);
        Assert.Equal(imageUrl, result.ImageUrl);
        Assert.Equal(deleteFlg, result.DeleteFlg);
        Assert.NotNull(result.ProductCategory);
        Assert.NotNull(result.ProductStock);
        Assert.Equal(10, result.ProductStock.Quantity);

        Log("正常な商品情報で Product を復元できることを確認しました。");
    }

    [Fact(DisplayName = "正常な商品情報でProductを新規作成できる")]
    public void Product_Create_ValidProduct_ReturnsProduct()
    {
        // Arrange
        Log("Product_Create_ValidProduct_ReturnsProduct：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(10);

        // Act
        var result = Product.Create(
            "エコバッグ",
            880,
            "https://example.com/images/bag.jpg",
            productCategory,
            productStock);

        // Assert
        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result.ProductUuid));
        Assert.Equal("エコバッグ", result.Name);
        Assert.Equal(880, result.Price);
        Assert.Equal("https://example.com/images/bag.jpg", result.ImageUrl);
        Assert.Equal(0, result.DeleteFlg);
        Assert.NotNull(result.ProductCategory);
        Assert.NotNull(result.ProductStock);

        Log("正常な商品情報で Product を新規作成できることを確認しました。");
    }

    [Fact(DisplayName = "価格が100万円以下の場合はProductを復元できる")]
    public void Product_Restore_PriceIsMaxValue_ReturnsProduct()
    {
        // Arrange
        Log("Product_Restore_PriceIsMaxValue_ReturnsProduct：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(5);

        // Act
        var result = Product.Restore(
            "45c24c9f-a494-4e75-afb8-794c5c66135f",
            "充電式マウス",
            1000000,
            "https://example.com/images/mouse.jpg",
            productCategory,
            0,
            productStock);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000000, result.Price);

        Log("価格が100万円の場合、Product を復元できることを確認しました。");
    }

    [Fact(DisplayName = "価格が100万円を超える場合はDomainExceptionが発生する")]
    public void Product_Restore_PriceIsOverLimit_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_PriceIsOverLimit_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(3);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "eb07baff-7f28-4356-abfb-020c31e04dc7",
                "Type-Cハブ",
                1000001,
                "https://example.com/images/hub.jpg",
                productCategory,
                0,
                productStock));

        Assert.Contains("価格は100万円以下", exception.Message);

        Log("価格が100万円を超える場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "価格が0円未満の場合はDomainExceptionが発生する")]
    public void Product_Restore_PriceIsNegative_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_PriceIsNegative_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(3);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "eb07baff-7f28-4356-abfb-020c31e04dc7",
                "Type-Cハブ",
                -1,
                "https://example.com/images/hub.jpg",
                productCategory,
                0,
                productStock));

        Assert.Contains("価格は0円以上", exception.Message);

        Log("価格が0円未満の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "商品名が空の場合はDomainExceptionが発生する")]
    public void Product_Restore_NameIsEmpty_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_NameIsEmpty_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(10);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "",
                880,
                "https://example.com/images/bag.jpg",
                productCategory,
                0,
                productStock));

        Assert.Contains("商品名を入力", exception.Message);

        Log("商品名が空の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "商品名が1文字の場合はDomainExceptionが発生する")]
    public void Product_Restore_NameIsTooShort_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_NameIsTooShort_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(10);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "あ",
                880,
                "https://example.com/images/bag.jpg",
                productCategory,
                0,
                productStock));

        Assert.Contains("商品名は2～20文字", exception.Message);

        Log("商品名が1文字の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "商品名が21文字の場合はDomainExceptionが発生する")]
    public void Product_Restore_NameIsTooLong_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_NameIsTooLong_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(10);

        var longName = "あいうえおかきくけこさしすせそたちつてとあ";

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                longName,
                880,
                "https://example.com/images/bag.jpg",
                productCategory,
                0,
                productStock));

        Assert.Contains("商品名は2～20文字", exception.Message);

        Log("商品名が21文字の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "画像URLが空の場合はDomainExceptionが発生する")]
    public void Product_Restore_ImageUrlIsEmpty_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_ImageUrlIsEmpty_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock(20);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                "耐水ノート",
                450,
                "",
                productCategory,
                0,
                productStock));

        Assert.Contains("画像をアップロード", exception.Message);

        Log("画像URLが空の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "商品カテゴリがnullの場合はDomainExceptionが発生する")]
    public void Product_Restore_CategoryIsNull_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_CategoryIsNull_ThrowsDomainException：テスト開始");

        var productStock = CreateProductStock(20);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                "耐水ノート",
                450,
                "https://example.com/images/note.jpg",
                null!,
                0,
                productStock));

        Assert.Contains("カテゴリを選択", exception.Message);

        Log("商品カテゴリが null の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "商品在庫がnullの場合はDomainExceptionが発生する")]
    public void Product_Restore_StockIsNull_ThrowsDomainException()
    {
        // Arrange
        Log("Product_Restore_StockIsNull_ThrowsDomainException：テスト開始");

        var productCategory = CreateProductCategory();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() =>
            Product.Restore(
                "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                "耐水ノート",
                450,
                "https://example.com/images/note.jpg",
                productCategory,
                0,
                null!));

        Assert.Contains("在庫数を入力", exception.Message);

        Log("商品在庫が null の場合、DomainException が発生することを確認しました。");
    }

    [Fact(DisplayName = "Deleteを実行すると削除フラグが1になる")]
    public void Product_Delete_SetsDeleteFlgToOne()
    {
        // Arrange
        Log("Product_Delete_SetsDeleteFlgToOne：テスト開始");

        var product = CreateProduct(deleteFlg: 0);

        // Act
        product.Delete();

        // Assert
        Assert.Equal(1, product.DeleteFlg);

        Log("Delete を実行すると DeleteFlg が 1 になることを確認しました。");
    }

    [Fact(DisplayName = "DeleteFlgが1の場合はIsDeleteがtrueを返す")]
    public void Product_IsDelete_DeleteFlgIsOne_ReturnsTrue()
    {
        // Arrange
        Log("Product_IsDelete_DeleteFlgIsOne_ReturnsTrue：テスト開始");

        var product = CreateProduct(deleteFlg: 1);

        // Act
        var result = product.IsDelete();

        // Assert
        Assert.True(result);

        Log("DeleteFlg が 1 の場合、IsDelete が true を返すことを確認しました。");
    }

    [Fact(DisplayName = "DeleteFlgが0の場合はIsDeleteがfalseを返す")]
    public void Product_IsDelete_DeleteFlgIsZero_ReturnsFalse()
    {
        // Arrange
        Log("Product_IsDelete_DeleteFlgIsZero_ReturnsFalse：テスト開始");

        var product = CreateProduct(deleteFlg: 0);

        // Act
        var result = product.IsDelete();

        // Assert
        Assert.False(result);

        Log("DeleteFlg が 0 の場合、IsDelete が false を返すことを確認しました。");
    }

    [Fact(DisplayName = "商品名を変更できる")]
    public void Product_ChangeName_ValidName_ChangesName()
    {
        // Arrange
        Log("Product_ChangeName_ValidName_ChangesName：テスト開始");

        var product = CreateProduct(deleteFlg: 0);

        // Act
        product.ChangeName("新商品名");

        // Assert
        Assert.Equal("新商品名", product.Name);

        Log("商品名を変更できることを確認しました。");
    }

    [Fact(DisplayName = "価格を変更できる")]
    public void Product_ChangePrice_ValidPrice_ChangesPrice()
    {
        // Arrange
        Log("Product_ChangePrice_ValidPrice_ChangesPrice：テスト開始");

        var product = CreateProduct(deleteFlg: 0);

        // Act
        product.ChangePrice(1200);

        // Assert
        Assert.Equal(1200, product.Price);

        Log("価格を変更できることを確認しました。");
    }

    [Fact(DisplayName = "画像URLを変更できる")]
    public void Product_ChangeImageUrl_ValidImageUrl_ChangesImageUrl()
    {
        // Arrange
        Log("Product_ChangeImageUrl_ValidImageUrl_ChangesImageUrl：テスト開始");

        var product = CreateProduct(deleteFlg: 0);

        // Act
        product.ChangeImageUrl("https://example.com/images/new.jpg");

        // Assert
        Assert.Equal("https://example.com/images/new.jpg", product.ImageUrl);

        Log("画像URLを変更できることを確認しました。");
    }

    [Fact(DisplayName = "商品カテゴリを変更できる")]
    public void Product_ChangeCategory_ValidCategory_ChangesCategory()
    {
        // Arrange
        Log("Product_ChangeCategory_ValidCategory_ChangesCategory：テスト開始");

        var product = CreateProduct(deleteFlg: 0);
        var newCategory = CreateProductCategory(
            "22222222-2222-2222-2222-222222222222",
            "PC周辺機器");

        // Act
        product.ChangeCategory(newCategory);

        // Assert
        Assert.NotNull(product.ProductCategory);
        Assert.Equal("22222222-2222-2222-2222-222222222222", product.ProductCategory.CategoryUuid);
        Assert.Equal("PC周辺機器", product.ProductCategory.Name);

        Log("商品カテゴリを変更できることを確認しました。");
    }

    [Fact(DisplayName = "商品在庫を変更できる")]
    public void Product_ChangeStock_ValidStock_ChangesStock()
    {
        // Arrange
        Log("Product_ChangeStock_ValidStock_ChangesStock：テスト開始");

        var product = CreateProduct(deleteFlg: 0);
        var newStock = CreateProductStock(50);

        // Act
        product.ChangeStock(newStock);

        // Assert
        Assert.NotNull(product.ProductStock);
        Assert.Equal(50, product.ProductStock.Quantity);

        Log("商品在庫を変更できることを確認しました。");
    }

    [Fact(DisplayName = "ProductStockを作成できる")]
    public void ProductStock_Create_ValidQuantity_ReturnsProductStock()
    {
        // Arrange
        Log("ProductStock_Create_ValidQuantity_ReturnsProductStock：テスト開始");

        // Act
        var result = CreateProductStock(10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Quantity);

        Log("ProductStock を作成できることを確認しました。");
    }

    [Fact(DisplayName = "ProductCategoryを作成できる")]
    public void ProductCategory_Create_ValidCategory_ReturnsProductCategory()
    {
        // Arrange
        Log("ProductCategory_Create_ValidCategory_ReturnsProductCategory：テスト開始");

        // Act
        var result = CreateProductCategory(
            "11111111-1111-1111-1111-111111111111",
            "雑貨");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("11111111-1111-1111-1111-111111111111", result.CategoryUuid);
        Assert.Equal("雑貨", result.Name);

        Log("ProductCategory を作成できることを確認しました。");
    }

    private static Product CreateProduct(int deleteFlg)
    {
        return Product.Restore(
            "b7af7239-108b-4698-b2a7-2fe4469b275a",
            "エコバッグ",
            880,
            "https://example.com/images/bag.jpg",
            CreateProductCategory(),
            deleteFlg,
            CreateProductStock(10));
    }

    private static ProductCategory CreateProductCategory()
    {
        return CreateProductCategory(
            "11111111-1111-1111-1111-111111111111",
            "雑貨");
    }

    private static ProductCategory CreateProductCategory(
        string categoryUuid,
        string name)
    {
        var category =
            (ProductCategory)FormatterServices.GetUninitializedObject(typeof(ProductCategory));

        SetProperty(category, nameof(ProductCategory.CategoryUuid), categoryUuid);
        SetProperty(category, nameof(ProductCategory.Name), name);

        return category;
    }

    private static ProductStock CreateProductStock(int quantity)
    {
        var stock =
            (ProductStock)FormatterServices.GetUninitializedObject(typeof(ProductStock));

        SetProperty(stock, nameof(ProductStock.Quantity), quantity);

        return stock;
    }

    private static void SetProperty<T>(
        T target,
        string propertyName,
        object value)
    {
        var property = typeof(T).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} に {propertyName} プロパティが見つかりません。");
        }

        property.SetValue(target, value);
    }
}