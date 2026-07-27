using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Presentation.Tests.Adapters;

[TestClass]
public class RegisterProductViewModelAdapterTests
{
    private const string ImageUrl =
        "https://example.com/photos/sample.png"; //石原:追加 Controllerから渡される画像URL

    private RegisterProductViewModelAdapter
        _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter =
            new RegisterProductViewModelAdapter();
    }

    /// <summary>
    /// 正常な商品登録リクエストを生成する
    /// </summary>
    /// <returns>
    /// 商品登録リクエスト
    /// </returns>
    private static RegisterProductRequest
        CreateValidRequest()
    {
        return new RegisterProductRequest
        {
            ProductName = "ボールペン",
            Price = 120,
            Stock = 50,
            CategoryUuid =
                Guid.NewGuid().ToString(),
        };
    }

    /// <summary>
    /// UT-REA-020
    /// ViewModelからProductへ正常に変換できること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_ReturnsProduct_WhenRequestIsValid()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                ImageUrl); //石原:変更 画像URLを別引数で渡す

        // Assert
        Assert.IsNotNull(product);

        Assert.AreEqual(
            request.ProductName,
            product.Name);

        Assert.AreEqual(
            request.Price,
            product.Price);

        Assert.AreEqual(
            ImageUrl,
            product.ImageUrl); //石原:変更 別引数の画像URLが設定されたことを確認する

        Assert.AreEqual(
            request.CategoryUuid,
            product.ProductCategory
                .CategoryUuid);

        Assert.AreEqual(
            request.Stock,
            product.ProductStock.Quantity);
    }

    /// <summary>
    /// UT-REA-021
    /// 商品名が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_ProductNameIsRestored()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.ProductName = "ノート";

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                ImageUrl); //石原:変更 画像URLを別引数で渡す

        // Assert
        Assert.AreEqual(
            "ノート",
            product.Name);
    }

    /// <summary>
    /// UT-REA-022
    /// 価格が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_PriceIsRestored()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Price = 500;

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                ImageUrl); //石原:変更 画像URLを別引数で渡す

        // Assert
        Assert.AreEqual(
            500,
            product.Price);
    }

    /// <summary>
    /// UT-REA-023
    /// 在庫数が正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_StockIsRestored()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        request.Stock = 999;

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                ImageUrl); //石原:変更 画像URLを別引数で渡す

        // Assert
        Assert.AreEqual(
            999,
            product.ProductStock.Quantity);
    }

    /// <summary>
    /// UT-REA-024
    /// カテゴリUUIDが正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_CategoryUuidIsRestored()
    {
        // Arrange
        string categoryUuid =
            Guid.NewGuid().ToString();

        RegisterProductRequest request =
            CreateValidRequest();

        request.CategoryUuid =
            categoryUuid;

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                ImageUrl); //石原:変更 画像URLを別引数で渡す

        // Assert
        Assert.AreEqual(
            categoryUuid,
            product.ProductCategory
                .CategoryUuid);
    }

    /// <summary>
    /// UT-REA-025
    /// 画像URLが正しく設定されること
    /// </summary>
    [TestMethod]
    public async Task
        RestoreAsync_ImageUrlIsRestored()
    {
        // Arrange
        RegisterProductRequest request =
            CreateValidRequest();

        const string expectedImageUrl =
            "https://example.com/photos/image.png";

        // Act
        var product =
            await _adapter.RestoreAsync(
                request,
                expectedImageUrl); //石原:変更 リクエストDTOではなく別引数で画像URLを渡す

        // Assert
        Assert.AreEqual(
            expectedImageUrl,
            product.ImageUrl);
    }
}