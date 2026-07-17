using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// 商品修正ユースケースの単体テスト。
/// </summary>
[TestClass]
public class UpdateProductUsecaseTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private Mock<IProductCategoryRepository> _productCategoryRepositoryMock = null!;
    private UpdateProductUsecase _usecase = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        _usecase = new UpdateProductUsecase(
            _unitOfWorkMock.Object,
            _productRepositoryMock.Object,
            _productCategoryRepositoryMock.Object);
    }

    /// <summary>
    /// 正常な商品修正情報の場合、商品情報を更新できること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_正常な修正内容の場合_商品情報を更新する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var oldCategory = CreateCategory("文房具");
        var newCategory = CreateCategory("生活雑貨");
        var product = CreateProduct(productUuid, oldCategory);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                oldCategory,
                newCategory
            });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        // Act
        var result = await _usecase.ExecuteAsync(
            productUuid,
            "修正商品",
            1500,
            20,
            newCategory.CategoryUuid,
            "https://example.com/update.png");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("修正商品", result.Name);
        Assert.AreEqual(1500, result.Price);
        Assert.AreEqual(20, result.ProductStock.Quantity);
        Assert.AreEqual(newCategory.CategoryUuid, result.ProductCategory.CategoryUuid);
        Assert.AreEqual("https://example.com/update.png", result.ImageUrl);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);

        _productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Once);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.RollbackAsync(),
            Times.Never);
    }

    /// <summary>
    /// 商品UUIDが空の場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品UUIDが空の場合_DomainExceptionを送出する()
    {
        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                "",
                "修正商品",
                1500,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品UUIDの形式が不正です。", exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(It.IsAny<string>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// 商品UUIDの形式が不正な場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品UUIDの形式が不正な場合_DomainExceptionを送出する()
    {
        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                "invalid-uuid",
                "修正商品",
                1500,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品UUIDの形式が不正です。", exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(It.IsAny<string>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// 商品が存在しない場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品が存在しない場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品が見つかりません。", exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);

        _productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// 指定したカテゴリが存在しない場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_カテゴリが存在しない場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);
        var notExistsCategoryUuid = Guid.NewGuid().ToString();

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                20,
                notExistsCategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("カテゴリを選択してください", exception.Message);

        _productRepositoryMock.Verify(
            repository => repository.SelectByUuidAsync(productUuid),
            Times.Once);

        _productCategoryRepositoryMock.Verify(
            repository => repository.SelectAllAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);
    }

    /// <summary>
    /// 商品名が空の場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品名が空の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "",
                1500,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品名を入力してください", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    /// <summary>
    /// 商品名が1文字の場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_商品名が1文字の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "A",
                1500,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品名は2～20文字で入力してください", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    /// <summary>
    /// 価格が100万円を超える場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_価格が100万円を超える場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000001,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("価格は100万円以下で入力してください", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    /// <summary>
    /// 在庫数が1000を超える場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_在庫数が1000を超える場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                1001,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("在庫数は1000個以下で入力してください", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    /// <summary>
    /// 画像URLが空の場合、DomainExceptionが送出されること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_画像URLが空の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                20,
                category.CategoryUuid,
                ""));

        // Assert
        Assert.AreEqual("画像をアップロードしてください", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Never);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Never);
    }

    /// <summary>
    /// Repositoryの更新結果がfalseの場合、Rollbackされること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_Repository更新結果がfalseの場合_Rollbackする()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(false);

        // Act
        var exception = await ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("商品情報を更新できませんでした。", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Once);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.RollbackAsync(),
            Times.Once);
    }

    /// <summary>
    /// Repository更新時に例外が発生した場合、Rollbackされること。
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_Repository更新時に例外が発生した場合_Rollbackする()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory("文房具");
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>
            {
                category
            });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ThrowsAsync(new InvalidOperationException("DB更新エラー"));

        // Act
        var exception = await ThrowsAsync<InvalidOperationException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1500,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.AreEqual("DB更新エラー", exception.Message);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.BeginTransactionAsync(),
            Times.Once);

        _productRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<Product>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.RollbackAsync(),
            Times.Once);
    }

    /// <summary>
    /// テスト用の商品カテゴリを生成する。
    /// </summary>
    private static ProductCategory CreateCategory(string name)
    {
        return new ProductCategory(
            Guid.NewGuid().ToString(),
            name);
    }

    /// <summary>
    /// テスト用の商品を生成する。
    /// </summary>
    private static Product CreateProduct(
        string productUuid,
        ProductCategory productCategory)
    {
        return Product.Restore(
            productUuid,
            "修正前商品",
            1000,
            "https://example.com/before.png",
            productCategory,
            0,
            ProductStock.Restore(Guid.NewGuid().ToString(), 10));
    }

    /// <summary>
    /// 指定した例外が送出されることを確認する。
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