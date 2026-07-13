using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// 商品修正ユースケースの単体テスト。
/// </summary>
public class UpdateProductUsecaseTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IProductCategoryRepository> _productCategoryRepositoryMock;

    private readonly UpdateProductUsecase _usecase;

    public UpdateProductUsecaseTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _productCategoryRepositoryMock = new Mock<IProductCategoryRepository>();

        _usecase = new UpdateProductUsecase(
            _unitOfWorkMock.Object,
            _productRepositoryMock.Object,
            _productCategoryRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_正常な商品修正情報の場合_商品情報が更新される()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var oldCategory = CreateCategory();
        var newCategory = CreateCategory();
        var product = CreateProduct(productUuid, oldCategory);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { oldCategory, newCategory });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(true);

        // Act
        await _usecase.ExecuteAsync(
            productUuid,
            "修正商品",
            1000,
            20,
            newCategory.CategoryUuid,
            "https://example.com/update.png");

        // Assert
        Assert.Equal("修正商品", product.Name);
        Assert.Equal(1000, product.Price);
        Assert.Equal(20, product.ProductStock.Quantity);
        Assert.Equal(newCategory.CategoryUuid, product.ProductCategory.CategoryUuid);
        Assert.Equal("https://example.com/update.png", product.ImageUrl);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Once);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_商品UUIDが空の場合_DomainExceptionを送出する()
    {
        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                "",
                "修正商品",
                1000,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品UUIDの形式が不正です。", exception.Message);

        _productRepositoryMock.Verify(repository => repository.SelectByUuidAsync(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_商品UUIDの形式が不正な場合_DomainExceptionを送出する()
    {
        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                "invalid-uuid",
                "修正商品",
                1000,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品UUIDの形式が不正です。", exception.Message);

        _productRepositoryMock.Verify(repository => repository.SelectByUuidAsync(It.IsAny<string>()), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_商品が存在しない場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync((Product?)null);

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品が見つかりません。", exception.Message);

        _productRepositoryMock.Verify(repository => repository.SelectByUuidAsync(productUuid), Times.Once);
        _productCategoryRepositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_カテゴリが存在しない場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var product = CreateProduct(productUuid, CreateCategory());

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>());

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                20,
                Guid.NewGuid().ToString(),
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("カテゴリを選択してください", exception.Message);

        _productRepositoryMock.Verify(repository => repository.SelectByUuidAsync(productUuid), Times.Once);
        _productCategoryRepositoryMock.Verify(repository => repository.SelectAllAsync(), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_商品名が空の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "",
                1000,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品名を入力してください", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_商品名が1文字の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "A",
                1000,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品名は2～20文字で入力してください", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_価格が100万円を超える場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000001,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("価格は100万円以下で入力してください", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_在庫数が1000を超える場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                1001,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("在庫数は1000個以下で入力してください", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_画像URLが空の場合_DomainExceptionを送出する()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                20,
                category.CategoryUuid,
                ""));

        // Assert
        Assert.Equal("画像をアップロードしてください", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Never);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_Repositoryの更新結果がfalseの場合_Rollbackする()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("商品情報を更新できませんでした。", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Once);
        _productRepositoryMock.Verify(repository => repository.UpdateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.RollbackAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Repository更新時に例外が発生した場合_Rollbackする()
    {
        // Arrange
        var productUuid = Guid.NewGuid().ToString();
        var category = CreateCategory();
        var product = CreateProduct(productUuid, category);

        _productRepositoryMock
            .Setup(repository => repository.SelectByUuidAsync(productUuid))
            .ReturnsAsync(product);

        _productCategoryRepositoryMock
            .Setup(repository => repository.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory> { category });

        _productRepositoryMock
            .Setup(repository => repository.UpdateAsync(It.IsAny<Product>()))
            .ThrowsAsync(new InvalidOperationException("DB更新エラー"));

        // Act
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _usecase.ExecuteAsync(
                productUuid,
                "修正商品",
                1000,
                20,
                category.CategoryUuid,
                "https://example.com/update.png"));

        // Assert
        Assert.Equal("DB更新エラー", exception.Message);

        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Never);
        _unitOfWorkMock.Verify(unitOfWork => unitOfWork.RollbackAsync(), Times.Once);
    }

    private static ProductCategory CreateCategory()
    {
        return new ProductCategory(
            Guid.NewGuid().ToString(),
            "文房具");
    }

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
}