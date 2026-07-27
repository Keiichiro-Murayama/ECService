using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;

namespace ECService.Presentation.Tests.Adapters;

/// <summary>
/// 商品詳細取得ViewModel変換アダプタの単体テスト。
/// </summary>
[TestClass]
public class GetProductViewModelAdapterTests
{
    private GetProductViewModelAdapter _adapter = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _adapter = new GetProductViewModelAdapter();
    }

    /// <summary>
    /// ProductをGetProductInfoResponseへ変換できること。
    /// </summary>
    [TestMethod]
    public void Convert_Productを渡した場合_GetProductInfoResponseへ変換する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var categoryUuid = Guid.NewGuid().ToString();

        var product = CreateProduct(
            productUuid,
            categoryUuid);

        // Act
        var result = _adapter.Convert(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(GetProductInfoResponse));

        Assert.AreEqual(productUuid, result.ProductUuid);
        Assert.AreEqual("詳細取得テスト商品", result.ProductName);
        Assert.AreEqual(1200, result.Price);
        Assert.AreEqual(15, result.Stock);
        Assert.AreEqual(categoryUuid, result.CategoryUuid);
        Assert.AreEqual("https://example.com/product.png", result.ImageUrl);
    }

    /// <summary>
    /// テスト用の商品を生成する。
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        string categoryUuid)
    {
        var category = new ProductCategory(
            categoryUuid,
            "文房具");

        var stock = ProductStock.Restore(
            Guid.NewGuid().ToString(),
            15);

        return Product.Restore(
            productUuid,
            "詳細取得テスト商品",
            1200,
            "https://example.com/product.png",
            category,
            0,
            stock);
    }
}