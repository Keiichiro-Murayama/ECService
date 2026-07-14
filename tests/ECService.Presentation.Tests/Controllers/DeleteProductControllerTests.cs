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
    /// テストで使用する正常な商品UUID
    /// </summary>
    private const string ProductUuid =
        "9374cfe6-bc67-4147-92e6-9f8afab3c06b";

    /// <summary>
    /// 商品削除が成功した場合、
    /// 200 OKと成功メッセージを返すことを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_DeleteSucceeds_ReturnsOk()
    {
        // Arrange
        var usecaseMock = new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(ProductUuid))
            .Returns(Task.CompletedTask);

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var result = await controller.DeleteProduct(ProductUuid);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(OkObjectResult));

        var okResult = (OkObjectResult)result;

        Assert.AreEqual(
            200,
            okResult.StatusCode);

        Assert.IsNotNull(okResult.Value);

        var messageProperty = okResult.Value
            .GetType()
            .GetProperty("message");

        Assert.IsNotNull(messageProperty);

        var actualMessage = messageProperty
            .GetValue(okResult.Value)?
            .ToString();

        Assert.AreEqual(
            "商品を削除しました。",
            actualMessage);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// 商品が存在しない場合、
    /// 404 Not Foundとエラーメッセージを返すことを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_ProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var usecaseMock = new Mock<IDeleteProductUsecase>();

        usecaseMock
            .Setup(usecase => usecase.ExecuteAsync(ProductUuid))
            .ThrowsAsync(
                new InvalidOperationException(
                    "指定された商品が見つかりません。"));

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var result = await controller.DeleteProduct(ProductUuid);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(NotFoundObjectResult));

        var notFoundResult =
            (NotFoundObjectResult)result;

        Assert.AreEqual(
            404,
            notFoundResult.StatusCode);

        Assert.IsNotNull(notFoundResult.Value);

        var messageProperty = notFoundResult.Value
            .GetType()
            .GetProperty("message");

        Assert.IsNotNull(messageProperty);

        var actualMessage = messageProperty
            .GetValue(notFoundResult.Value)?
            .ToString();

        Assert.AreEqual(
            "指定された商品が見つかりません。",
            actualMessage);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }

    /// <summary>
    /// 商品UUIDの形式が不正な場合、
    /// 400 Bad Requestを返すことを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_InvalidUuid_ReturnsBadRequest()
    {
        // Arrange
        const string invalidUuid = "invalid-uuid";

        var usecaseMock = new Mock<IDeleteProductUsecase>();

        var controller =
            new DeleteProductController(usecaseMock.Object);

        // Act
        var result = await controller.DeleteProduct(invalidUuid);

        // Assert
        Assert.IsInstanceOfType(
            result,
            typeof(BadRequestObjectResult));

        var badRequestResult =
            (BadRequestObjectResult)result;

        Assert.AreEqual(
            400,
            badRequestResult.StatusCode);

        Assert.IsNotNull(badRequestResult.Value);

        var messageProperty = badRequestResult.Value
            .GetType()
            .GetProperty("message");

        Assert.IsNotNull(messageProperty);

        var actualMessage = messageProperty
            .GetValue(badRequestResult.Value)?
            .ToString();

        Assert.AreEqual(
            "商品UUIDの形式が正しくありません。",
            actualMessage);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// Usecaseで予期しない例外が発生した場合、
    /// その例外が呼び出し元へ伝播することを確認する
    /// </summary>
    [TestMethod]
    public async Task DeleteProduct_UsecaseThrowsException_PropagatesException()
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
        var actualException =
            await Assert.ThrowsExactlyAsync<Exception>(
                () => controller.DeleteProduct(ProductUuid));

        // Assert
        Assert.AreSame(
            expectedException,
            actualException);

        usecaseMock.Verify(
            usecase => usecase.ExecuteAsync(ProductUuid),
            Times.Once);
    }
}