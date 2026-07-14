using System.Reflection;
using System.Runtime.Serialization;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;

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
    public TestContext TestContext { get; set; } = null!;

    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    [TestMethod]
    public void Convert_ProductList_ReturnsSearchProductsResponse()
    {
        // Arrange
        Log("Convert_ProductList_ReturnsSearchProductsResponse：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>
        {
            CreateProduct(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "エコバッグ",
                880,
                "https://example.com/images/bag.jpg")
        };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.AreEqual(1, result.Products.Count);

        Assert.AreEqual("b7af7239-108b-4698-b2a7-2fe4469b275a", result.Products[0].ProductUuid);
        Assert.AreEqual("エコバッグ", result.Products[0].ProductName);
        Assert.AreEqual(880, result.Products[0].Price);
        Assert.AreEqual("https://example.com/images/bag.jpg", result.Products[0].ImageUrl);

        Log("Product 1件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

    [TestMethod]
    public void Convert_MultipleProducts_ReturnsMultipleProductsItems()
    {
        // Arrange
        Log("Convert_MultipleProducts_ReturnsMultipleProductsItems：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>
        {
            CreateProduct(
                "b7af7239-108b-4698-b2a7-2fe4469b275a",
                "エコバッグ",
                880,
                "https://example.com/images/bag.jpg"),

            CreateProduct(
                "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
                "耐水ノート",
                450,
                "https://example.com/images/note.jpg")
        };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Products);
        Assert.AreEqual(2, result.Products.Count);

        Assert.AreEqual("エコバッグ", result.Products[0].ProductName);
        Assert.AreEqual("耐水ノート", result.Products[1].ProductName);

        Log("Product 複数件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

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
        Assert.AreEqual(0, result.Products.Count);

        Log("空の List<Product> を渡した場合、空の products が返ることを確認しました。");
    }

    [TestMethod]
    public void Convert_Product_MapsEachPropertyCorrectly()
    {
        // Arrange
        Log("Convert_Product_MapsEachPropertyCorrectly：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var product = CreateProduct(
            "45c24c9f-a494-4e75-afb8-794c5c66135f",
            "充電式マウス",
            2480,
            "https://example.com/images/mouse.jpg");

        var products = new List<Product> { product };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Products.Count);

        var item = result.Products[0];

        Assert.AreEqual(product.ProductUuid, item.ProductUuid);
        Assert.AreEqual(product.Name, item.ProductName);
        Assert.AreEqual(product.Price, item.Price);
        Assert.AreEqual(product.ImageUrl, item.ImageUrl);

        Log("Product の ProductUuid、Name、Price、ImageUrl が正しく ProductsItem に詰め替えられることを確認しました。");
    }

    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product =
            (Product)FormatterServices.GetUninitializedObject(typeof(Product));

        SetProperty(product, nameof(Product.ProductUuid), productUuid);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.ImageUrl), imageUrl);

        return product;
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