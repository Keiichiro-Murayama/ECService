using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.Controllers;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

[TestClass]
public class GetCategoriesControllerTests
{
    private Mock<IGetCategoriesUsecase> _usecaseMock = null!;
    private GetCategoriesViewModelAdapter _adapter = null!;
    private GetCategoriesController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _usecaseMock = new Mock<IGetCategoriesUsecase>();
        _adapter = new GetCategoriesViewModelAdapter();

        _controller = new GetCategoriesController(
            _usecaseMock.Object,
            _adapter);
    }

    /// <summary>
    /// カテゴリ一覧を正常に取得できること
    /// </summary>
    [TestMethod]
    public async Task GetCategories_ReturnOk_WithCategories()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            ProductCategory.Create("食品"),
            ProductCategory.Create("飲料"),
            ProductCategory.Create("雑貨")
        };

        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(categories);

        // Act
        var result = await _controller.GetCategories();

        // Assert
        // var okResult = result as OkObjectResult;
        var okResult = result.Result as OkObjectResult; // 修正: ActionResult<T> の場合、Result プロパティを使用して OkObjectResult にアクセスする必要があります。

        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var response = okResult.Value as GetCategoriesResponse;

        Assert.IsNotNull(response);
        Assert.HasCount(3, response.Categories);
        Assert.AreEqual(categories[0].CategoryUuid, response.Categories[0].CategoryUuid);
        Assert.AreEqual(categories[0].Name, response.Categories[0].Name);

        _usecaseMock.Verify(x => x.ExecuteAsync(), Times.Once);
    }

    /// <summary>
    /// カテゴリが0件の場合でも正常終了すること
    /// </summary>
    [TestMethod]
    public async Task GetCategories_ReturnOk_WithEmptyCategories()
    {
        // Arrange
        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(new List<ProductCategory>());

        // Act
        var result = await _controller.GetCategories();

        // Assert
        var okResult = result.Result as OkObjectResult; // 修正: ActionResult<T> の場合、Result プロパティを使用して OkObjectResult にアクセスする必要があります。

        Assert.IsNotNull(okResult);
        Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);

        var response = okResult.Value as GetCategoriesResponse;

        Assert.IsNotNull(response);
        Assert.IsEmpty(response.Categories);


    }

    /// <summary>
    /// Usecaseで例外が発生した場合、例外が送出されること
    /// </summary>
    [TestMethod]
    public async Task GetCategories_ThrowException()
    {
        // Arrange
        _usecaseMock
            .Setup(x => x.ExecuteAsync())
            .ThrowsAsync(new Exception("DB Error"));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(
            async () => await _controller.GetCategories());

        Assert.AreEqual("DB Error", ex.Message);
    }
}