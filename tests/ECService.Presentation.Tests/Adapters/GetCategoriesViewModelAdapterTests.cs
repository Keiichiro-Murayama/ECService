using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Presentation.Tests.Adapters;

[TestClass]
public class GetCategoriesViewModelAdapterTests
{
    private GetCategoriesViewModelAdapter _adapter = null!;

    [TestInitialize]
    public void Initialize()
    {
        _adapter = new GetCategoriesViewModelAdapter();
    }

    /// <summary>
    /// カテゴリ一覧をViewModelへ変換できること
    /// </summary>
    [TestMethod]
    public async Task Convert_ReturnResponse_WithCategories()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            ProductCategory.Create("食品"),
            ProductCategory.Create("飲料"),
            ProductCategory.Create("雑貨")
        };

        // Act
        var result = await _adapter.Convert(categories);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Categories);
        Assert.HasCount(3, result.Categories);

        Assert.AreEqual(categories[0].CategoryUuid, result.Categories[0].CategoryUuid);
        Assert.AreEqual(categories[0].Name, result.Categories[0].Name);

        Assert.AreEqual(categories[1].CategoryUuid, result.Categories[1].CategoryUuid);
        Assert.AreEqual(categories[1].Name, result.Categories[1].Name);

        Assert.AreEqual(categories[2].CategoryUuid, result.Categories[2].CategoryUuid);
        Assert.AreEqual(categories[2].Name, result.Categories[2].Name);
    }

    /// <summary>
    /// 空のカテゴリ一覧をViewModelへ変換できること
    /// </summary>
    [TestMethod]
    public async Task Convert_ReturnResponse_WithEmptyCategories()
    {
        // Arrange
        var categories = new List<ProductCategory>();

        // Act
        var result = await _adapter.Convert(categories);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Categories);
        Assert.IsEmpty(result.Categories);
    }

    /// <summary>
    /// 1件のカテゴリをViewModelへ変換できること
    /// </summary>
    [TestMethod]
    public async Task Convert_ReturnResponse_WithOneCategory()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            ProductCategory.Create("食品")
        };

        // Act
        var result = await _adapter.Convert(categories);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Categories);
        Assert.HasCount(1, result.Categories);

        Assert.AreEqual(categories[0].CategoryUuid, result.Categories[0].CategoryUuid);
        Assert.AreEqual(categories[0].Name, result.Categories[0].Name);
    }
}