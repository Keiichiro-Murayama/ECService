using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Controllers;
using Microsoft.AspNetCore.Http;
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
    /// 商品削除が成功した場合、
    /// 200 OKと成功メッセージが返ること
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_DeleteSucceeds_ReturnsOk()
    {
        // Arrange
        Log(
            "DeleteProduct_DeleteSucceeds_ReturnsOk："
            + "テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var usecaseMock =
            new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(productUuid))
            .Returns(Task.CompletedTask);

        var controller =
            new DeleteProductController(
                usecaseMock.Object);

        // Act
        var actionResult =
            await controller.DeleteProduct(productUuid);

        // Assert
        var okResult =
            actionResult as OkObjectResult;

        Assert.IsNotNull(okResult);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        Assert.AreEqual(
            "商品を削除しました。",
            GetMessage(okResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(productUuid),
            Times.Once);

        Log(
            "商品削除が成功した場合、"
            + "200 OKと成功メッセージが返ることを確認しました。");
    }

    /// <summary>
    /// 商品が存在しない場合、
    /// 404 Not Foundとエラーメッセージが返ること
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        Log(
            "DeleteProduct_ProductDoesNotExist_ReturnsNotFound："
            + "テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var usecaseMock =
            new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(productUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    "指定された商品が見つかりません。"));

        var controller =
            new DeleteProductController(
                usecaseMock.Object);

        // Act
        var actionResult =
            await controller.DeleteProduct(productUuid);

        // Assert
        var notFoundResult =
            actionResult as NotFoundObjectResult;

        Assert.IsNotNull(notFoundResult);

        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            notFoundResult.StatusCode);

        Assert.AreEqual(
            "指定された商品が見つかりません。",
            GetMessage(notFoundResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(productUuid),
            Times.Once);

        Log(
            "商品が存在しない場合、"
            + "404 Not Foundとエラーメッセージが"
            + "返ることを確認しました。");
    }

    /// <summary>
    /// 商品UUIDがUUID形式ではない場合、
    /// 400 Bad Requestが返ること
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_InvalidUuid_ReturnsBadRequest()
    {
        // Arrange
        Log(
            "DeleteProduct_InvalidUuid_ReturnsBadRequest："
            + "テスト開始");

        const string invalidProductUuid =
            "invalid-uuid";

        var usecaseMock =
            new Mock<IDeleteProductUsecase>();

        var controller =
            new DeleteProductController(
                usecaseMock.Object);

        // Act
        var actionResult =
            await controller.DeleteProduct(
                invalidProductUuid);

        // Assert
        var badRequestResult =
            actionResult as BadRequestObjectResult;

        Assert.IsNotNull(badRequestResult);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            badRequestResult.StatusCode);

        Assert.AreEqual(
            "商品UUIDの形式が正しくありません。",
            GetMessage(badRequestResult.Value));

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(
                    It.IsAny<string>()),
            Times.Never);

        Log(
            "商品UUIDがUUID形式ではない場合、"
            + "400 Bad Requestが返り、"
            + "Usecaseが呼ばれないことを確認しました。");
    }

    /// <summary>
    /// Usecaseで予期しない例外が発生した場合、
    /// 同じ例外が呼び出し元へ伝播すること
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_UnexpectedException_PropagatesException()
    {
        // Arrange
        Log(
            "DeleteProduct_UnexpectedException_"
            + "PropagatesException：テスト開始");

        const string productUuid =
            "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

        var expectedException =
            new Exception(
                "予期しないエラーが発生しました。");

        var usecaseMock =
            new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase =>
                usecase.ExecuteAsync(productUuid))
            .ThrowsAsync(expectedException);

        var controller =
            new DeleteProductController(
                usecaseMock.Object);

        // Act
        var actualException =
            await Assert.ThrowsExactlyAsync<Exception>(
                async () =>
                    await controller.DeleteProduct(
                        productUuid));

        // Assert
        Assert.AreSame(
            expectedException,
            actualException);

        Assert.AreEqual(
            "予期しないエラーが発生しました。",
            actualException.Message);

        usecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(productUuid),
            Times.Once);

        Log(
            "Usecaseで発生した同一のExceptionが、"
            + "呼び出し元へ伝播することを確認しました。");
    }

    /// <summary>
    /// 匿名オブジェクトのmessageプロパティを取得する
    /// </summary>
    private static string? GetMessage(
        object? response)
    {
        if (response is null)
        {
            return null;
        }

        var messageProperty =
            response
                .GetType()
                .GetProperty("message");

        return messageProperty?
            .GetValue(response)?
            .ToString();
    }
}