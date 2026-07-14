using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Presentation.Tests.Controllers;

/// <summary>
/// DeleteProductControllerの単体テスト
/// </summary>
[TestClass]
public class DeleteProductControllerTests
{
    /// <summary>
    /// テスト対象の商品UUID
    /// </summary>
    private const string ProductUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    /// <summary>
    /// Usecaseが正常終了した場合、
    /// 204 No Contentが返ることを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteAsync_DeleteSucceeds_ReturnsNoContent()
    {
        // Arrange
        var usecaseMock = new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(ProductUuid))
            .Returns(Task.CompletedTask);

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var result = await controller.DeleteAsync(ProductUuid);

        // Assert
        Assert.IsInstanceOfType<NoContentResult>(result);

        var noContentResult = (NoContentResult)result;

        Assert.AreEqual(204, noContentResult.StatusCode);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// Usecaseで商品不存在の例外が発生した場合、
    /// InvalidOperationExceptionが呼び出し元へ伝播することを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteAsync_ProductDoesNotExist_PropagatesInvalidOperationException()
    {
        // Arrange
        var usecaseMock = new Mock<IDeleteProductUsecase>();

        var expectedException = new InvalidOperationException(
            "削除対象の商品が見つかりませんでした。");

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(ProductUuid))
            .ThrowsAsync(expectedException);

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var actualException =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => controller.DeleteAsync(ProductUuid));

        // Assert
        Assert.AreSame(expectedException, actualException);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// Usecaseで予期しない例外が発生した場合、
    /// その例外が呼び出し元へ伝播することを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteAsync_UsecaseThrowsException_PropagatesException()
    {
        // Arrange
        var usecaseMock = new Mock<IDeleteProductUsecase>();

        var expectedException =
            new Exception("予期しないエラーが発生しました。");

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(ProductUuid))
            .ThrowsAsync(expectedException);

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var actualException = await Assert.ThrowsExactlyAsync<Exception>(
            () => controller.DeleteAsync(ProductUuid));

        // Assert
        Assert.AreSame(expectedException, actualException);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }
}