using System.Reflection;
using System.Runtime.Serialization;
using ECService.Domain.Adapters;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// ProductFactory の単体テスト
///
/// ProductEntity、ProductStockEntity、ProductCategoryEntity から
/// Product ドメインオブジェクトへ正しく復元できることを検証する。
/// </summary>
public class ProductFactoryTests
{
    private readonly ITestOutputHelper _output;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="output">テスト出力用</param>
    public ProductFactoryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// ターミナルとテスト結果にログを出力する
    /// </summary>
    /// <param name="message">出力メッセージ</param>
    private void Log(string message)
    {
        Console.WriteLine(message);
        _output.WriteLine(message);
    }

    /// <summary>
    /// 正常なEntityからProductドメインへ復元できること
    /// </summary>
    [Fact(DisplayName = "正常なEntityからProductドメインへ復元できる")]
    public async Task Factory_ValidEntities_ReturnsProduct()
    {
        // Arrange
        Log("Factory_ValidEntities_ReturnsProduct：テスト開始");

        var factory = CreateFactory();

        var categoryEntity = CreateCategoryEntity();

        var productEntity = new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("b7af7239-108b-4698-b2a7-2fe4469b275a"),
            ProductCategoryId = 1,
            Name = "エコバッグ",
            Price = 880,
            ImageUrl = "https://example.com/images/bag.jpg",
            DeleteFlag = 0,
            ProductCategory = categoryEntity
        };

        var stockEntity = new ProductStockEntity
        {
            Id = 1,
            ProductId = 1,
            Quantity = 10
        };

        // Act
        var result = await factory.Factory(
            productEntity,
            stockEntity,
            categoryEntity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("b7af7239-108b-4698-b2a7-2fe4469b275a", result.ProductUuid);
        Assert.Equal("エコバッグ", result.Name);
        Assert.Equal(880, result.Price);
        Assert.Equal("https://example.com/images/bag.jpg", result.ImageUrl);
        Assert.Equal(0, result.DeleteFlg);
        Assert.NotNull(result.ProductStock);
        Assert.Equal(10, result.ProductStock.Quantity);

        Log("Factory_ValidEntities_ReturnsProduct：EntityからProductドメインへ正しく復元できることを確認しました。");
    }

    /// <summary>
    /// 商品価格が100万円以下の場合、Productドメインへ復元できること
    /// </summary>
    [Fact(DisplayName = "商品価格が100万円以下の場合はProductドメインへ復元できる")]
    public async Task Factory_PriceIsWithinLimit_ReturnsProduct()
    {
        // Arrange
        Log("Factory_PriceIsWithinLimit_ReturnsProduct：テスト開始");

        var factory = CreateFactory();

        var categoryEntity = CreateCategoryEntity();

        var productEntity = new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("45c24c9f-a494-4e75-afb8-794c5c66135f"),
            ProductCategoryId = 1,
            Name = "充電式ワイヤレスマウス",
            Price = 1000000,
            ImageUrl = "https://example.com/images/mouse.jpg",
            DeleteFlag = 0,
            ProductCategory = categoryEntity
        };

        var stockEntity = new ProductStockEntity
        {
            Id = 1,
            ProductId = 1,
            Quantity = 5
        };

        // Act
        var result = await factory.Factory(
            productEntity,
            stockEntity,
            categoryEntity);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1000000, result.Price);

        Log("Factory_PriceIsWithinLimit_ReturnsProduct：商品価格が100万円以下の場合、Productドメインへ復元できることを確認しました。");
    }

    /// <summary>
    /// 商品価格が100万円を超える場合、DomainExceptionが発生すること
    /// </summary>
    [Fact(DisplayName = "商品価格が100万円を超える場合はDomainExceptionが発生する")]
    public async Task Factory_PriceIsOverLimit_ThrowsDomainException()
    {
        // Arrange
        Log("Factory_PriceIsOverLimit_ThrowsDomainException：テスト開始");

        var factory = CreateFactory();

        var categoryEntity = CreateCategoryEntity();

        var productEntity = new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("eb07baff-7f28-4356-abfb-020c31e04dc7"),
            ProductCategoryId = 1,
            Name = "Type-C ハブ 6in1",
            Price = 1000001,
            ImageUrl = "https://example.com/images/hub.jpg",
            DeleteFlag = 0,
            ProductCategory = categoryEntity
        };

        var stockEntity = new ProductStockEntity
        {
            Id = 1,
            ProductId = 1,
            Quantity = 3
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            factory.Factory(productEntity, stockEntity, categoryEntity));

        Assert.Contains("価格", exception.Message);

        Log("Factory_PriceIsOverLimit_ThrowsDomainException：商品価格が100万円を超える場合、DomainException が発生することを確認しました。");
    }

    /// <summary>
    /// 画像URLが空の場合、DomainExceptionが発生すること
    /// </summary>
    [Fact(DisplayName = "画像URLが空の場合はDomainExceptionが発生する")]
    public async Task Factory_ImageUrlIsEmpty_ThrowsDomainException()
    {
        // Arrange
        Log("Factory_ImageUrlIsEmpty_ThrowsDomainException：テスト開始");

        var factory = CreateFactory();

        var categoryEntity = CreateCategoryEntity();

        var productEntity = new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("9374cfe6-bc67-4147-92e6-9f8afab3c06b"),
            ProductCategoryId = 1,
            Name = "耐水ノート(A5)",
            Price = 450,
            ImageUrl = "",
            DeleteFlag = 0,
            ProductCategory = categoryEntity
        };

        var stockEntity = new ProductStockEntity
        {
            Id = 1,
            ProductId = 1,
            Quantity = 20
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            factory.Factory(productEntity, stockEntity, categoryEntity));

        Assert.Contains("画像", exception.Message);

        Log("Factory_ImageUrlIsEmpty_ThrowsDomainException：画像URLが空の場合、DomainException が発生することを確認しました。");
    }

    /// <summary>
    /// 在庫数をProductStockへ復元できること
    /// </summary>
    [Fact(DisplayName = "在庫数をProductStockへ復元できる")]
    public async Task Factory_StockQuantity_MapsToProductStock()
    {
        // Arrange
        Log("Factory_StockQuantity_MapsToProductStock：テスト開始");

        var factory = CreateFactory();

        var categoryEntity = CreateCategoryEntity();

        var productEntity = new ProductEntity
        {
            Id = 1,
            ProductUuid = Guid.Parse("4a53c109-cf79-47f3-8426-3aaf1ad29b71"),
            ProductCategoryId = 1,
            Name = "アロマキャンドル",
            Price = 1500,
            ImageUrl = "https://example.com/images/candle.jpg",
            DeleteFlag = 0,
            ProductCategory = categoryEntity
        };

        var stockEntity = new ProductStockEntity
        {
            Id = 1,
            ProductId = 1,
            Quantity = 25
        };

        // Act
        var result = await factory.Factory(
            productEntity,
            stockEntity,
            categoryEntity);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ProductStock);
        Assert.Equal(25, result.ProductStock.Quantity);

        Log("Factory_StockQuantity_MapsToProductStock：ProductStock の Quantity が正しく復元されることを確認しました。");
    }

    /// <summary>
    /// テスト用のProductFactoryを作成する
    /// </summary>
    /// <returns>ProductFactory</returns>
    private static ProductFactory CreateFactory()
    {
        var categoryRestorerMock =
            new Mock<IRestorer<ProductCategory, ProductCategoryEntity>>();

        var stockRestorerMock =
            new Mock<IRestorer<ProductStock, ProductStockEntity>>();

        categoryRestorerMock
            .Setup(restorer => restorer.RestoreAsync(It.IsAny<ProductCategoryEntity>()))
            .ReturnsAsync((ProductCategoryEntity entity) =>
                CreateProductCategory(
                    entity.CategoryUuid.ToString(),
                    entity.Name));

        stockRestorerMock
            .Setup(restorer => restorer.RestoreAsync(It.IsAny<ProductStockEntity>()))
            .ReturnsAsync((ProductStockEntity entity) =>
                CreateProductStock(entity.Quantity));

        return new ProductFactory(
            categoryRestorerMock.Object,
            stockRestorerMock.Object);
    }

    /// <summary>
    /// テスト用の商品カテゴリEntityを作成する
    /// </summary>
    /// <returns>商品カテゴリEntity</returns>
    private static ProductCategoryEntity CreateCategoryEntity()
    {
        return new ProductCategoryEntity
        {
            Id = 1,
            CategoryUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "雑貨"
        };
    }

    /// <summary>
    /// テスト用のProductCategoryを作成する
    /// </summary>
    /// <param name="categoryUuid">カテゴリUUID</param>
    /// <param name="name">カテゴリ名</param>
    /// <returns>ProductCategory</returns>
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

    /// <summary>
    /// テスト用のProductStockを作成する
    /// </summary>
    /// <param name="quantity">在庫数</param>
    /// <returns>ProductStock</returns>
    private static ProductStock CreateProductStock(int quantity)
    {
        var stock =
            (ProductStock)FormatterServices.GetUninitializedObject(typeof(ProductStock));

        SetProperty(stock, nameof(ProductStock.Quantity), quantity);

        return stock;
    }

    /// <summary>
    /// private set のプロパティにテスト用の値を設定する
    /// </summary>
    /// <typeparam name="T">対象型</typeparam>
    /// <param name="target">対象オブジェクト</param>
    /// <param name="propertyName">プロパティ名</param>
    /// <param name="value">設定値</param>
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