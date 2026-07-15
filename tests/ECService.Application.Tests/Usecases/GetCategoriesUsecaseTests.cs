using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

[TestClass]
public class GetCategoriesUsecaseTests
{
    private Mock<IUnitOfWork> _unitOfWorkMock = null!;
    private Mock<IProductCategoryRepository> _repositoryMock = null!;
    private GetCategoriesUsecase _usecase = null!;

    [TestInitialize]
    public void Initialize()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IProductCategoryRepository>();

        _usecase = new GetCategoriesUsecase(
            _unitOfWorkMock.Object,
            _repositoryMock.Object);
    }

    /// <summary>
    /// カテゴリ一覧を取得できること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnCategories()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            ProductCategory.Create("食品"),
            ProductCategory.Create("飲料"),
            ProductCategory.Create("雑貨")
        };

        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count);

        CollectionAssert.AreEqual(
            categories.Select(c => c.CategoryUuid).ToList(),
            result.Select(c => c.CategoryUuid).ToList());

        CollectionAssert.AreEqual(
            categories.Select(c => c.Name).ToList(),
            result.Select(c => c.Name).ToList());

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// カテゴリが0件の場合、空のリストが返ること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnEmptyList()
    {
        // Arrange
        var categories = new List<ProductCategory>();

        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
        _repositoryMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、例外が送出されること
    /// </summary>
    [TestMethod]
public async Task ExecuteAsync_RepositoryThrowsException()
{
    // Arrange
    _repositoryMock
        .Setup(x => x.SelectAllAsync())
        .ThrowsAsync(new Exception("DB Error"));

    // Act
    Exception? exception = null;

    try
    {
        await _usecase.ExecuteAsync();
    }
    catch (Exception ex)
    {
        exception = ex;
    }

    

    // Assert
    Assert.IsNotNull(exception);
    Assert.AreEqual("DB Error", exception.Message);

    _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
    _repositoryMock.VerifyNoOtherCalls();
}
}