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
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual(categories[0].CategoryUuid, result[0].CategoryUuid);
        Assert.AreEqual(categories[0].Name, result[0].Name);

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
    }

    /// <summary>
    /// カテゴリが0件の場合、空のリストが返ること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ReturnEmptyList()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.SelectAllAsync())
            .ReturnsAsync(new List<ProductCategory>());

        // Act
        var result = await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);

        _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
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
    try
    {
        await _usecase.ExecuteAsync();
        Assert.Fail("例外が発生しませんでした。");
    }
    catch (Exception ex)
    {
        // Assert
        Assert.AreEqual("DB Error", ex.Message);
    }

    _repositoryMock.Verify(x => x.SelectAllAsync(), Times.Once);
}
}