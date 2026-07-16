using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Adapters;

/// <summary>
/// ProductCategoryEntityAdapterの単体テスト
/// </summary>
[TestClass]
public class ProductCategoryEntityAdapterTests
{
    [TestMethod(DisplayName = "UT-ADP-007 ProductCategoryドメインをProductCategoryEntityへ変換できる")]
    public async Task ConvertAsync_ShouldConvertProductCategoryDomainToEntity()
    {
        // Arrange
        var adapter = new ProductCategoryEntityAdapter();

        var category = new ProductCategory(
            "11111111-1111-1111-1111-111111111111",
            "文房具");

        // Act
        var entity = await adapter.ConvertAsync(category);

        // Assert
        Assert.AreEqual(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            entity.CategoryUuid);

        Assert.AreEqual("文房具", entity.Name);
    }

    [TestMethod(DisplayName = "UT-ADP-008 ProductCategoryEntityをProductCategoryドメインへ復元できる")]
    public async Task RestoreAsync_ShouldRestoreProductCategoryEntityToDomain()
    {
        // Arrange
        var adapter = new ProductCategoryEntityAdapter();

        var entity = new ProductCategoryEntity
        {
            Id = 1,
            CategoryUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "文房具"
        };

        // Act
        var category = await adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual(
            "11111111-1111-1111-1111-111111111111",
            category.CategoryUuid);

        Assert.AreEqual("文房具", category.Name);
    }

    [TestMethod(DisplayName = "UT-ADP-009 カテゴリ名を正しく復元できる")]
    public async Task RestoreAsync_ShouldRestoreCategoryName()
    {
        // Arrange
        var adapter = new ProductCategoryEntityAdapter();

        var entity = new ProductCategoryEntity
        {
            Id = 1,
            CategoryUuid = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Name = "家電"
        };

        // Act
        var category = await adapter.RestoreAsync(entity);

        // Assert
        Assert.AreEqual("家電", category.Name);
    }
}