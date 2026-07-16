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
    /// テスト結果出力用
    /// </summary>
    public TestContext TestContext { get; set; } = null!;

    /// <summary>
    /// コンソールとテスト結果へメッセージを出力する
    /// </summary>
    private void Log(string message)
    {
        Console.WriteLine(message);
        TestContext.WriteLine(message);
    }

    /// <summary>
    /// Repositoryがtrueを返した場合、
    /// 商品削除処理が正常終了すること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_DeleteSucceeds_CompletesNormally()
    {
        // Arrange
        Log(
            "ExecuteAsync_DeleteSucceeds_CompletesNormally："
            + "テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var repositoryMock =
            new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository =>
                repository.DeleteAsync(productUuid))
            .ReturnsAsync(true);

        var usecase =
            new DeleteProductUsecase(repositoryMock.Object);

        // Act
        await usecase.ExecuteAsync(productUuid);

        // Assert
        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(productUuid),
            Times.Once);

        Log(
            "Repositoryがtrueを返した場合、"
            + "例外なく削除処理が完了することを確認しました。");
    }

    /// <summary>
    /// Repositoryがfalseを返した場合、
    /// InvalidOperationExceptionが発生すること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_ProductDoesNotExist_ThrowsInvalidOperationException()
    {
        // Arrange
        Log(
            "ExecuteAsync_ProductDoesNotExist_"
            + "ThrowsInvalidOperationException：テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var repositoryMock =
            new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository =>
                repository.DeleteAsync(productUuid))
            .ReturnsAsync(false);

        var usecase =
            new DeleteProductUsecase(repositoryMock.Object);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                    await usecase.ExecuteAsync(productUuid));

        // Assert
        Assert.AreEqual(
            "指定された商品が見つかりません。",
            exception.Message);

        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(productUuid),
            Times.Once);

        Log(
            "Repositoryがfalseを返した場合、"
            + "InvalidOperationExceptionが発生することを確認しました。");
    }

    /// <summary>
    /// Repositoryで予期しない例外が発生した場合、
    /// 同じ例外が呼び出し元へ伝播すること
    /// </summary>
    [TestMethod]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        Log(
            "ExecuteAsync_RepositoryThrowsException_"
            + "PropagatesException：テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var expectedException =
            new Exception(
                "Repositoryでエラーが発生しました。");

        var repositoryMock =
            new Mock<IProductRepository>();

        repositoryMock
            .Setup(repository =>
                repository.DeleteAsync(productUuid))
            .ThrowsAsync(expectedException);

        var usecase =
            new DeleteProductUsecase(repositoryMock.Object);

        // Act
        var actualException =
            await Assert.ThrowsExactlyAsync<Exception>(
                async () =>
                    await usecase.ExecuteAsync(productUuid));

        // Assert
        Assert.AreSame(
            expectedException,
            actualException);

        Assert.AreEqual(
            "Repositoryでエラーが発生しました。",
            actualException.Message);

        repositoryMock.Verify(
            repository =>
                repository.DeleteAsync(productUuid),
            Times.Once);

        Log(
            "Repositoryで発生した同一のExceptionが、"
            + "呼び出し元へ伝播することを確認しました。");
    }
}