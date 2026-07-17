using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Presentation.Tests.Adapters;

[TestClass]
public class RegisterProductViewModelAdapterTests
{
    private RegisterProductViewModelAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new RegisterProductViewModelAdapter();
    }

    /// <summary>
    /// UT-REA-020
    /// ViewModelからProductへ正常に変換できること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnsProduct_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterProductRequest
        {
            ProductName = "ボールペン",
            Price = 120,
            Stock = 50,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "sample.png"
        };

        // Act
        var product = await _adapter.RestoreAsync(request);

        // Assert
        Assert.IsNotNull(product);
        Assert.AreEqual(request.ProductName, product.Name);
        Assert.AreEqual(request.Price, product.Price);
        Assert.AreEqual(request.ImageUrl, product.ImageUrl);
        Assert.AreEqual(request.CategoryUuid, product.ProductCategory.CategoryUuid);
        Assert.AreEqual(request.Stock, product.ProductStock.Quantity);
    }

    /// <summary>
    /// UT-REA-021
    /// 商品名が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ProductNameIsRestored()
    {
        var request = new RegisterProductRequest
        {
            ProductName = "ノート",
            Price = 100,
            Stock = 10,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "note.png"
        };

        var product = await _adapter.RestoreAsync(request);

        Assert.AreEqual("ノート", product.Name);
    }

    /// <summary>
    /// UT-REA-022
    /// 価格が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_PriceIsRestored()
    {
        var request = new RegisterProductRequest
        {
            ProductName = "ノート",
            Price = 500,
            Stock = 10,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "note.png"
        };

        var product = await _adapter.RestoreAsync(request);

        Assert.AreEqual(500, product.Price);
    }

    /// <summary>
    /// UT-REA-023
    /// 在庫数が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_StockIsRestored()
    {
        var request = new RegisterProductRequest
        {
            ProductName = "ノート",
            Price = 500,
            Stock = 999,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "note.png"
        };

        var product = await _adapter.RestoreAsync(request);

        Assert.AreEqual(999, product.ProductStock.Quantity);
    }

    /// <summary>
    /// UT-REA-024
    /// カテゴリUUIDが正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_CategoryUuidIsRestored()
    {
        var categoryUuid = Guid.NewGuid().ToString();

        var request = new RegisterProductRequest
        {
            ProductName = "ノート",
            Price = 500,
            Stock = 10,
            CategoryUuid = categoryUuid,
            ImageUrl = "note.png"
        };

        var product = await _adapter.RestoreAsync(request);

        Assert.AreEqual(categoryUuid, product.ProductCategory.CategoryUuid);
    }

    /// <summary>
    /// UT-REA-025
    /// 画像URLが正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ImageUrlIsRestored()
    {
        var request = new RegisterProductRequest
        {
            ProductName = "ノート",
            Price = 500,
            Stock = 10,
            CategoryUuid = Guid.NewGuid().ToString(),
            ImageUrl = "image.png"
        };

        var product = await _adapter.RestoreAsync(request);

        Assert.AreEqual("image.png", product.ImageUrl);
    }
}