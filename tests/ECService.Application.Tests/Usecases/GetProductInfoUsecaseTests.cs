using ECService.Application.Usecases.Imps;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// 商品詳細取得ユースケースの単体テスト。
/// </summary>
[TestClass]
public class GetProductInfoUsecaseTests
{
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private GetProductInfoUsecase _usecase = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _usecase = new GetProductInfoUsecase(_productRepositoryMock.Object);
    }

    /// <summary>
    /// 商品が存在する場合、商品情報を返すこと。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品が存在する場合_商品情報を返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var product = CreateProduct(productUuid);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        // Act
        var result = await _usecase.ExecuteAsync(productUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(productUuid, result.ProductUuid);
        Assert.AreEqual("詳細取得テスト商品", result.Name);
        Assert.AreEqual(1200, result.Price);
        Assert.AreEqual(15, result.ProductStock.Quantity);
        Assert.AreEqual("https://example.com/product.png", result.ImageUrl);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// 商品が存在しない場合、nullを返すこと。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品が存在しない場合_Nullを返す()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _usecase.ExecuteAsync(productUuid);

        // Assert
        Assert.IsNull(result);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、例外がそのまま送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_Repositoryで例外が発生した場合_例外を送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ThrowsAsync(new InvalidOperationException("Repository error"));

        // Act
        var exception = await ThrowsAsync<InvalidOperationException>(
            () => _usecase.ExecuteAsync(productUuid));

        // Assert
        Assert.AreEqual("Repository error", exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);
    }

    /// <summary>
    /// テスト用の商品を生成する。
    /// </summary>
    private static Product CreateProduct(string productUuid)
    {
        var category = new ProductCategory(
            Guid.NewGuid().ToString(),
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

    /// <summary>
    /// 指定した例外が送出されることを検証する。
    /// </summary>
    private static async Task<TException> ThrowsAsync<TException>(Func<Task> action)
        where TException : Exception
    {
        try
        {
            await action();
        }
        catch (TException exception)
        {
            return exception;
        }
        catch (Exception exception)
        {
            Assert.Fail(
                $"想定した例外は {typeof(TException).Name} でしたが、実際は {exception.GetType().Name} でした。");
        }

        Assert.Fail($"想定した例外 {typeof(TException).Name} が送出されませんでした。");
        return null!;
    }
}