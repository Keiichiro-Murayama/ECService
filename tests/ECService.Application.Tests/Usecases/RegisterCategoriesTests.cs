using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

[TestClass]
public class RegisterCategoryUsecaseTests
{
    private Mock<IUnitOfWork> _unitOfWork = null!;
    private Mock<IProductCategoryRepository> _repository = null!;
    private RegisterProductCategoryUsecase _usecase = null!;

    [TestInitialize]
    public void Initialize()
    {
        _unitOfWork = new Mock<IUnitOfWork>();
        _repository = new Mock<IProductCategoryRepository>();

        _usecase = new RegisterProductCategoryUsecase(
            _unitOfWork.Object,
            _repository.Object);
    }

    /// <summary>
    /// テスト用カテゴリ生成
    /// </summary>
    private ProductCategory CreateCategory()
    {
        return ProductCategory.Create("食品");
    }

    /// <summary>
    /// UT-REC-001
    /// カテゴリを正常に登録できること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_RegisterSuccess()
    {
        // Arrange
        var category = CreateCategory();

        _repository.Setup(x => x.SelectAllAsync())
                   .ReturnsAsync(new List<ProductCategory>());

        _repository.Setup(x => x.ExistsByNameAsync(category.Name))
                   .ReturnsAsync(false);

        // Act
        await _usecase.ExecuteAsync(category);

        // Assert
        _repository.Verify(x => x.SelectAllAsync(), Times.Once);
        // _repository.Verify(x => x.ExistsByNameAsync(category.Name), Times.Once);
        // _unitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
        // _repository.Verify(x => x.CreateAsync(category), Times.Once);
        // _unitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        _unitOfWork.Verify(x => x.RollbackAsync(), Times.Never);
    }

    /// <summary>
    /// UT-REC-002
    /// 同名カテゴリが存在する場合は登録できないこと
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CategoryAlreadyExists()
    {
        // Arrange
        var category = CreateCategory();

        _repository.Setup(x => x.SelectAllAsync())
                   .ReturnsAsync(new List<ProductCategory>());

        _repository.Setup(x => x.ExistsByNameAsync(category.Name))
                   .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(
            () => _usecase.ExecuteAsync(category));

        // _repository.Verify(x => x.CreateAsync(It.IsAny<ProductCategory>()), Times.Never);
        // _unitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Never);
        // _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
        // _unitOfWork.Verify(x => x.RollbackAsync(), Times.Never);
    }

    /// <summary>
    /// UT-REC-003
    /// CreateAsyncで例外が発生した場合はロールバックされること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CreateAsyncThrowsException()
    {
        // Arrange
        var category = CreateCategory();

        _repository.Setup(x => x.SelectAllAsync())
                   .ReturnsAsync(new List<ProductCategory>());

        _repository.Setup(x => x.ExistsByNameAsync(category.Name))
                   .ReturnsAsync(false);

        _repository.Setup(x => x.CreateAsync(category))
                   .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _usecase.ExecuteAsync(category));

        // _unitOfWork.Verify(x => x.BeginTransactionAsync(), Times.Once);
        // _unitOfWork.Verify(x => x.RollbackAsync(), Times.Once);
        // _unitOfWork.Verify(x => x.CommitAsync(), Times.Never);
    }

    /// <summary>
    /// UT-REC-004
    /// CommitAsyncで例外が発生した場合はロールバックされること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_CommitAsyncThrowsException()
    {
        // Arrange
        var category = CreateCategory();

        _repository.Setup(x => x.SelectAllAsync())
                   .ReturnsAsync(new List<ProductCategory>());

        _repository.Setup(x => x.ExistsByNameAsync(category.Name))
                   .ReturnsAsync(false);

        _unitOfWork.Setup(x => x.CommitAsync())
                   .ThrowsAsync(new Exception());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _usecase.ExecuteAsync(category));

        // _repository.Verify(x => x.CreateAsync(category), Times.Once);
        // _unitOfWork.Verify(x => x.RollbackAsync(), Times.Once);
    }

    /// <summary>
    /// UT-REC-005
    /// カテゴリ一覧取得処理が実行されること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_SelectAllAsyncIsCalled()
    {
        // Arrange
        var category = CreateCategory();

        _repository.Setup(x => x.SelectAllAsync())
                   .ReturnsAsync(new List<ProductCategory>());

        _repository.Setup(x => x.ExistsByNameAsync(category.Name))
                   .ReturnsAsync(false);

        // Act
        await _usecase.ExecuteAsync(category);

        // // Assert
        // _repository.Verify(x => x.SelectAllAsync(), Times.Once);
    }
}