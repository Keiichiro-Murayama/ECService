using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

[TestClass]
public class RegisterProductUsecaseTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private RegisterProductUsecase _usecase = null!;


    [TestInitialize]
    public void Initialize()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _productRepositoryMock = new Mock<IProductRepository>();

        _usecase = new RegisterProductUsecase(
            _unitOfWorkMock.Object,
            _productRepositoryMock.Object);
    }


    /// <summary>
    /// テスト用商品生成
    /// </summary>
    private Product CreateProduct()
    {
        var category = ProductCategory.Create("食品");

        var productStock = ProductStock.Create(50);

        return Product.Create(
            "ボールペン",
            120,
            "http://example.com/image.png",
            category,
            productStock);
    }



    /// <summary>
    /// UT-REA-001
    /// 商品を正常に登録できること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_RegisterSuccess()
    {
        // Arrange
        var product = CreateProduct();


        _productRepositoryMock
            .Setup(x => x.ExistsByNameAsync(product.Name))
            .ReturnsAsync(false);


        // Act
        await _usecase.ExecuteAsync(product);


        // Assert

        _productRepositoryMock.Verify(
            x => x.ExistsByNameAsync(product.Name),
            Times.Once);


        _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);


        _productRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Product>()),
            Times.Once);


        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Once);


        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }



    /// <summary>
    /// UT-REA-002
    /// 同名商品が存在する場合、登録できないこと
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ProductAlreadyExists()
    {
        // Arrange
        var product = CreateProduct();


        _productRepositoryMock
            .Setup(x => x.ExistsByNameAsync(product.Name))
            .ReturnsAsync(true);


        // Act
        Exception? exception = null;

        try
        {
            await _usecase.ExecuteAsync(product);
        }
        catch(Exception ex)
        {
            exception = ex;
        }


        // Assert

        Assert.IsInstanceOfType(
            exception,
            typeof(DomainException));


        _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Never);


        _productRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Product>()),
            Times.Never);


        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);


        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }



    /// <summary>
/// UT-REA-003
/// 商品登録時に例外が発生した場合、ロールバックされること
/// </summary>
[TestMethod]
public async Task ExecuteAsync_CreateException_Rollback()
{
    // Arrange
    var product = CreateProduct();


    _productRepositoryMock
        .Setup(x => x.ExistsByNameAsync(product.Name))
        .ReturnsAsync(false);


    // ★ここが重要
    // CreateAsyncで必ず例外を発生させる
    _productRepositoryMock
        .Setup(x => x.CreateAsync(It.IsAny<Product>()))
        .ThrowsAsync(new Exception("登録エラー"));



    // Act
    Exception? exception = null;

    try
    {
        await _usecase.ExecuteAsync(product);
    }
    catch(Exception ex)
    {
        exception = ex;
    }



    // Assert

    // throw; まで到達した確認
    Assert.IsNotNull(exception);


    // トランザクション開始確認
    _unitOfWorkMock.Verify(
        x => x.BeginTransactionAsync(),
        Times.Once);



    // CreateAsync実行確認
    _productRepositoryMock.Verify(
        x => x.CreateAsync(It.IsAny<Product>()),
        Times.Once);



    // ★ここでRollback確認
    _unitOfWorkMock.Verify(
        x => x.RollbackAsync(),
        Times.Once);



    // Commitされていないこと
    _unitOfWorkMock.Verify(
        x => x.CommitAsync(),
        Times.Never);
}



    /// <summary>
    /// UT-REA-004
    /// Commit時に例外発生した場合ロールバックされること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CommitException_Rollback()
    {
        // Arrange
        var product = CreateProduct();


        _productRepositoryMock
            .Setup(x => x.ExistsByNameAsync(product.Name))
            .ReturnsAsync(false);


        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .ThrowsAsync(new Exception("Commit Error"));



        // Act

        Exception? exception = null;

        try
        {
            await _usecase.ExecuteAsync(product);
        }
        catch(Exception ex)
        {
            exception = ex;
        }



        // Assert

        Assert.IsNotNull(exception);


        _unitOfWorkMock.Verify(
            x => x.BeginTransactionAsync(),
            Times.Once);


        _productRepositoryMock.Verify(
            x => x.CreateAsync(product),
            Times.Once);


        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Once);


        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);
    }
}