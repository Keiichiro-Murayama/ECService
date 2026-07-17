using System.Reflection;
using System.Runtime.CompilerServices;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Presentation.Tests.Adapters;

/// <summary>
/// SearchProductsViewModelAdapter の単体テスト
///
/// Product ドメインオブジェクトの一覧を、
/// 商品検索レスポンス用の SearchProductsResponse に正しく変換できることを検証する。
/// </summary>
[TestClass]
public class SearchProductsViewModelAdapterTests
{
    /// <summary>
    /// テスト出力用
    /// </summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// ターミナルとテスト結果にログを出力する
    /// </summary>
    /// <param name="message">出力メッセージ</param>
    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    /// <summary>
    /// Product一覧1件をSearchProductsResponseへ変換できること
    /// </summary>
    [TestMethod]
    public void Convert_ProductList_ReturnsSearchProductsResponse()
    {
        // Arrange
        Log("Convert_ProductList_ReturnsSearchProductsResponse：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
                name: "エコバッグ",
                price: 880,
                imageUrl: "https://example.com/images/bag.jpg")
        };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.HasCount(1, result.Products);

        Assert.AreEqual("b7af7239-108b-4698-b2a7-2fe4469b275a", result.Products[0].ProductUuid);
        Assert.AreEqual("エコバッグ", result.Products[0].ProductName);
        Assert.AreEqual(880, result.Products[0].Price);
        Assert.AreEqual("https://example.com/images/bag.jpg", result.Products[0].ImageUrl);

        Log("Product 1件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

    /// <summary>
    /// Product一覧複数件をSearchProductsResponseへ変換できること
    /// </summary>
    [TestMethod]
    public void Convert_MultipleProducts_ReturnsMultipleProductsItems()
    {
        // Arrange
        Log("Convert_MultipleProducts_ReturnsMultipleProductsItems：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>
        {
            CreateProduct(
                productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
                name: "エコバッグ",
                price: 880,
                imageUrl: "https://example.com/images/bag.jpg"),

            CreateProduct(
                productUuid: "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                name: "耐水ノート",
                price: 450,
                imageUrl: "https://example.com/images/note.jpg")
        };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.HasCount(2, result.Products);

        Assert.AreEqual("エコバッグ", result.Products[0].ProductName);
        Assert.AreEqual("耐水ノート", result.Products[1].ProductName);

        Log("Product 複数件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

    /// <summary>
    /// 空の商品一覧を渡した場合、空のproductsが返ること
    /// </summary>
    [TestMethod]
    public void Convert_EmptyProductList_ReturnsEmptyProducts()
    {
        // Arrange
        Log("Convert_EmptyProductList_ReturnsEmptyProducts：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>();

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.IsEmpty(result.Products);

        Log("空の List<Product> を渡した場合、空の products が返ることを確認しました。");
    }

    /// <summary>
    /// Productの各項目がProductsItemへ正しく詰め替えられること
    /// </summary>
    [TestMethod]
    public void Convert_Product_MapsEachPropertyCorrectly()
    {
        // Arrange
        Log("Convert_Product_MapsEachPropertyCorrectly：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var product = CreateProduct(
            productUuid: "45c24c9f-a494-4e75-afb8-794c5c66135f",
            name: "充電式マウス",
            price: 2480,
            imageUrl: "https://example.com/images/mouse.jpg");

        var products = new List<Product> { product };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.HasCount(1, result.Products);

        var item = result.Products[0];

        Assert.AreEqual(product.ProductUuid, item.ProductUuid);
        Assert.AreEqual(product.Name, item.ProductName);
        Assert.AreEqual(product.Price, item.Price);
        Assert.AreEqual(product.ImageUrl, item.ImageUrl);

        Log("Product の ProductUuid、Name、Price、ImageUrl が正しく ProductsItem に詰め替えられることを確認しました。");
    }

    /// <summary>
    /// テスト用のProductを作成する
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product =
            (Product)RuntimeHelpers.GetUninitializedObject(typeof(Product));

        SetProperty(product, nameof(Product.ProductUuid), productUuid);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.ImageUrl), imageUrl);

        return product;
    }

    /// <summary>
    /// private set のプロパティにテスト用の値を設定する
    /// </summary>
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