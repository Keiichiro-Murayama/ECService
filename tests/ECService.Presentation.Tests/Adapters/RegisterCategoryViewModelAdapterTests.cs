using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ECService.Domain.Models;

namespace ECService.Presentation.Tests.Adapters;

[TestClass]
public class RegisterCategoryViewModelAdapterTests
{
    private RegisterCategoryViewModelAdapter _adapter = null!;


    [TestInitialize]
    public void Initialize()
    {
        _adapter = new RegisterCategoryViewModelAdapter();
    }


    /// <summary>
    /// UT-REC-018
    /// RequestをProductCategoryへ正常変換できること
    /// </summary>
    [TestMethod]
    public async Task RestoreAsync_ReturnProductCategory_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterCategoryRequest
        {
            CategoryName = "文房具"
        };


        // Act
        var result = await _adapter.RestoreAsync(request);


        // Assert
        Assert.IsNotNull(result);

        Assert.AreEqual(
            "文房具",
            result.Name);

        Assert.IsFalse(
            string.IsNullOrEmpty(result.CategoryUuid));
    }
}