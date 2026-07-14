// using ECService.Application.Usecases.Imps;
// using ECService.Domain.Models;
// using ECService.Domain.Repositories;
// using Moq;

// namespace ECService.Application.Tests.Usecases;

// /// <summary>
// /// SearchProductsUsecase の単体テスト
// ///
// /// 商品リポジトリから商品一覧を取得する。
// /// カテゴリUUIDが未指定・空文字・空白の場合は全商品を取得する。
// /// カテゴリUUIDが指定されている場合は、指定カテゴリの商品一覧を取得する。
// /// 検索結果が0件でも、エラーではなく空のリストを返す。
// /// 読み取りのみのため、トランザクションは用いない。
// /// </summary>
// public class SearchProductsUsecaseTests
// {
//     /// <summary>
//     /// カテゴリUUIDが未指定・空文字・空白の場合、
//     /// 全商品取得処理 SelectAllAsync が呼ばれること
//     /// </summary>
//     /// <param name="categoryUuid">カテゴリUUID</param>
//     [Theory]
//     [InlineData(null)]
//     [InlineData("")]
//     [InlineData("   ")]
//     public async Task ExecuteAsync_CategoryUuidIsEmpty_CallsSelectAllAsync(
//         string? categoryUuid)
//     {
//         // Arrange
//         var productRepositoryMock = new Mock<IProductRepository>();

//         var products = new List<Product>();

//         productRepositoryMock
//             .Setup(repository => repository.SelectAllAsync())
//             .ReturnsAsync(products);

//         var usecase = new SearchProductsUsecase(
//             productRepositoryMock.Object);

//         // Act
//         var result = await usecase.ExecuteAsync(categoryUuid);

//         // Assert
//         productRepositoryMock.Verify(
//             repository => repository.SelectAllAsync(),
//             Times.Once);

//         productRepositoryMock.Verify(
//             repository => repository.SelectByCategoryAsync(It.IsAny<string>()),
//             Times.Never);

//         Assert.Equal(products, result);
//     }

//     /// <summary>
//     /// カテゴリUUIDが指定されている場合、
//     /// カテゴリ別商品取得処理 SelectByCategoryAsync が呼ばれること
//     /// </summary>
//     [Fact]
//     public async Task ExecuteAsync_CategoryUuidExists_CallsSelectByCategoryAsync()
//     {
//         // Arrange
//         var productRepositoryMock = new Mock<IProductRepository>();

//         var categoryUuid = "11111111-1111-1111-1111-111111111111";
//         var products = new List<Product>();

//         productRepositoryMock
//             .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
//             .ReturnsAsync(products);

//         var usecase = new SearchProductsUsecase(
//             productRepositoryMock.Object);

//         // Act
//         var result = await usecase.ExecuteAsync(categoryUuid);

//         // Assert
//         productRepositoryMock.Verify(
//             repository => repository.SelectByCategoryAsync(categoryUuid),
//             Times.Once);

//         productRepositoryMock.Verify(
//             repository => repository.SelectAllAsync(),
//             Times.Never);

//         Assert.Equal(products, result);
//     }

//     /// <summary>
//     /// 全商品検索の結果が0件の場合、
//     /// エラーではなく空のリストが返ること
//     /// </summary>
//     [Fact]
//     public async Task ExecuteAsync_SelectAllReturnsEmptyList_ReturnsEmptyList()
//     {
//         // Arrange
//         var productRepositoryMock = new Mock<IProductRepository>();

//         var products = new List<Product>();

//         productRepositoryMock
//             .Setup(repository => repository.SelectAllAsync())
//             .ReturnsAsync(products);

//         var usecase = new SearchProductsUsecase(
//             productRepositoryMock.Object);

//         // Act
//         var result = await usecase.ExecuteAsync(null);

//         // Assert
//         Assert.NotNull(result);
//         Assert.Empty(result);

//         productRepositoryMock.Verify(
//             repository => repository.SelectAllAsync(),
//             Times.Once);
//     }

//     /// <summary>
//     /// カテゴリ検索の結果が0件の場合、
//     /// エラーではなく空のリストが返ること
//     /// </summary>
//     [Fact]
//     public async Task ExecuteAsync_SelectByCategoryReturnsEmptyList_ReturnsEmptyList()
//     {
//         // Arrange
//         var productRepositoryMock = new Mock<IProductRepository>();

//         var categoryUuid = "99999999-9999-9999-9999-999999999999";
//         var products = new List<Product>();

//         productRepositoryMock
//             .Setup(repository => repository.SelectByCategoryAsync(categoryUuid))
//             .ReturnsAsync(products);

//         var usecase = new SearchProductsUsecase(
//             productRepositoryMock.Object);

//         // Act
//         var result = await usecase.ExecuteAsync(categoryUuid);

//         // Assert
//         Assert.NotNull(result);
//         Assert.Empty(result);

//         productRepositoryMock.Verify(
//             repository => repository.SelectByCategoryAsync(categoryUuid),
//             Times.Once);
//     }
// }