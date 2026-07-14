using ECService.Application.Usecases.Imps;
using ECService.Domain.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ECService.Application.Tests.Usecases;

/// <summary>
/// DeleteProductUsecaseの単体テスト
/// </summary>
[TestClass]
public class DeleteProductUsecaseTests
{
    /// <summary>
    /// テスト対象の商品UUID
    /// </summary>
    private const string ProductUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    /// <summary>
    /// Repositoryがtrueを返した場合、
    /// 例外なく処理が完了することを確認する
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_DeleteSucceeds_CompletesNormally()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.DeleteAsync(ProductUuid))
            .ReturnsAsync(true);

        var usecase = new DeleteProductUsecase(repositoryMock.Object);

        // Act
        await usecase.ExecuteAsync(ProductUuid);

        // Assert
        repositoryMock.Verify(
            repository => repository.DeleteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// Repositoryがfalseを返した場合、
    /// InvalidOperationExceptionが発生することを確認する
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ProductDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository => repository.DeleteAsync(ProductUuid))
            .ReturnsAsync(false);

        var usecase = new DeleteProductUsecase(repositoryMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                () => usecase.ExecuteAsync(ProductUuid));

        // Assert
        Assert.AreEqual(
            "指定された商品が見つかりません。",
            exception.Message);

        repositoryMock.Verify(
            repository => repository.DeleteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、
    /// その例外が呼び出し元へ伝播することを確認する
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();

        var expectedException =
            new Exception("Repositoryでエラーが発生しました。");

        repositoryMock
            .Setup(repository => repository.DeleteAsync(ProductUuid))
            .ThrowsAsync(expectedException);

        var usecase = new DeleteProductUsecase(repositoryMock.Object);

        // Act
        var actualException =
            await Assert.ThrowsExactlyAsync<Exception>(
                () => usecase.ExecuteAsync(ProductUuid));

        // Assert
        Assert.AreSame(expectedException, actualException);

        repositoryMock.Verify(
            repository => repository.DeleteAsync(ProductUuid),
            Times.Once);
    }
}