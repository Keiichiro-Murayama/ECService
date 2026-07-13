using System.Reflection;
using System.Runtime.Serialization;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using Xunit;
using Xunit.Abstractions;

namespace ECService.Presentation.Tests.Adapters;

/// <summary>
/// SearchProductsViewModelAdapter の単体テスト
///
/// Product ドメインオブジェクトの一覧を、
/// 商品検索レスポンス用の SearchProductsResponse に正しく変換できることを検証する。
/// </summary>
public class SearchProductsViewModelAdapterTests
{
    private readonly ITestOutputHelper _output;

    public SearchProductsViewModelAdapterTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// テスト出力用
    /// </summary>
    /// <param name="message">出力メッセージ</param>
    private void Log(string message)
    {
        Console.WriteLine(message);
        _output.WriteLine(message);
    }

    [Fact(DisplayName = "Product一覧1件をSearchProductsResponseへ変換できる")]
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
        Assert.NotNull(result);
        Assert.NotNull(result.Products);
        Assert.Single(result.Products);

        Assert.Equal("b7af7239-108b-4698-b2a7-2fe4469b275a", result.Products[0].ProductUuid);
        Assert.Equal("エコバッグ", result.Products[0].ProductName);
        Assert.Equal(880, result.Products[0].Price);
        Assert.Equal("https://example.com/images/bag.jpg", result.Products[0].ImageUrl);

        Log("Product 1件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

    [Fact(DisplayName = "Product一覧複数件をSearchProductsResponseへ変換できる")]
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
                name: "耐水ノート(A5)",
                price: 450,
                imageUrl: "https://example.com/images/note.jpg")
        };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Products);
        Assert.Equal(2, result.Products.Count);

        Assert.Equal("エコバッグ", result.Products[0].ProductName);
        Assert.Equal("耐水ノート(A5)", result.Products[1].ProductName);

        Log("Product 複数件を SearchProductsResponse へ正しく変換できることを確認しました。");
    }

    [Fact(DisplayName = "空の商品一覧を渡した場合は空のproductsが返る")]
    public void Convert_EmptyProductList_ReturnsEmptyProducts()
    {
        // Arrange
        Log("Convert_EmptyProductList_ReturnsEmptyProducts：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var products = new List<Product>();

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Products);
        Assert.Empty(result.Products);

        Log("空の List<Product> を渡した場合、空の products が返ることを確認しました。");
    }

    [Fact(DisplayName = "Productの各項目がProductsItemへ正しく詰め替えられる")]
    public void Convert_Product_MapsEachPropertyCorrectly()
    {
        // Arrange
        Log("Convert_Product_MapsEachPropertyCorrectly：テスト開始");

        var adapter = new SearchProductsViewModelAdapter();

        var product = CreateProduct(
            productUuid: "45c24c9f-a494-4e75-afb8-794c5c66135f",
            name: "充電式ワイヤレスマウス",
            price: 2480,
            imageUrl: "https://example.com/images/mouse.jpg");

        var products = new List<Product> { product };

        // Act
        var result = adapter.Convert(products);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Products);

        var item = result.Products[0];

        Assert.Equal(product.ProductUuid, item.ProductUuid);
        Assert.Equal(product.Name, item.ProductName);
        Assert.Equal(product.Price, item.Price);
        Assert.Equal(product.ImageUrl, item.ImageUrl);

        Log("Product の ProductUuid、Name、Price、ImageUrl が正しく ProductsItem に詰め替えられることを確認しました。");
    }

    /// <summary>
    /// テスト用のProductを作成する。
    /// Productのプロパティが private set のため、テスト用にリフレクションで値を設定する。
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        string name,
        int price,
        string imageUrl)
    {
        var product = (Product)FormatterServices.GetUninitializedObject(typeof(Product));

        SetProperty(product, nameof(Product.ProductUuid), productUuid);
        SetProperty(product, nameof(Product.Name), name);
        SetProperty(product, nameof(Product.Price), price);
        SetProperty(product, nameof(Product.ImageUrl), imageUrl);

        return product;
    }

    /// <summary>
    /// private set のプロパティにテスト用の値を設定する。
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