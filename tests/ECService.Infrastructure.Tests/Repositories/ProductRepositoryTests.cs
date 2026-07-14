// using System.Reflection;
// using System.Runtime.Serialization;
// using ECService.Domain.Adapters;
// using ECService.Domain.Models;
// using ECService.Infrastructure.Adapters;
// using ECService.Infrastructure.Contexts;
// using ECService.Infrastructure.Entities;
// using ECService.Infrastructure.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Xunit;
// using Xunit.Abstractions;

// namespace ECService.Infrastructure.Tests.Repositories;

// /// <summary>
// /// ProductRepository の単体テスト
// ///
// /// テスト用の InMemory DB を使用し、商品検索機能で使用する
// /// SelectAllAsync と SelectByCategoryAsync の動作を検証する。
// /// </summary>
// public class ProductRepositoryTests
// {
//     private readonly ITestOutputHelper _output;

//     /// <summary>
//     /// コンストラクタ
//     /// </summary>
//     /// <param name="output">テスト出力用</param>
//     public ProductRepositoryTests(ITestOutputHelper output)
//     {
//         _output = output;
//     }

//     /// <summary>
//     /// ターミナルとテスト結果にログを出力する
//     /// </summary>
//     /// <param name="message">出力メッセージ</param>
//     private void Log(string message)
//     {
//         Console.WriteLine(message);
//         _output.WriteLine(message);
//     }

//     /// <summary>
//     /// SelectAllAsync で削除されていない商品だけ取得できること
//     /// </summary>
//     [Fact(DisplayName = "SelectAllAsyncで削除されていない商品だけ取得できる")]
//     public async Task SelectAllAsync_ActiveProductsOnly_ReturnsProducts()
//     {
//         // Arrange
//         Log("SelectAllAsync_ActiveProductsOnly_ReturnsProducts：テスト開始");

//         using var context = CreateDbContext();

//         var category = CreateCategoryEntity(
//             id: 1,
//             categoryUuid: "11111111-1111-1111-1111-111111111111",
//             name: "雑貨");

//         var activeProduct1 = CreateProductEntity(
//             id: 1,
//             productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
//             categoryId: category.Id,
//             name: "エコバッグ",
//             price: 880,
//             imageUrl: "https://example.com/images/bag.jpg",
//             deleteFlag: 0,
//             category: category);

//         var deletedProduct = CreateProductEntity(
//             id: 2,
//             productUuid: "3fd7d44e-7cac-444b-b747-44eb988a0421",
//             categoryId: category.Id,
//             name: "高級ボールペン",
//             price: 1200,
//             imageUrl: "https://example.com/images/pen.jpg",
//             deleteFlag: 1,
//             category: category);

//         var activeProduct2 = CreateProductEntity(
//             id: 3,
//             productUuid: "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
//             categoryId: category.Id,
//             name: "耐水ノート(A5)",
//             price: 450,
//             imageUrl: "https://example.com/images/note.jpg",
//             deleteFlag: 0,
//             category: category);

//         await context.ProductCategories.AddAsync(category);
//         await context.Products.AddRangeAsync(activeProduct1, deletedProduct, activeProduct2);
//         await context.ProductStocks.AddRangeAsync(
//             CreateStockEntity(id: 1, productId: activeProduct1.Id, quantity: 10),
//             CreateStockEntity(id: 2, productId: deletedProduct.Id, quantity: 5),
//             CreateStockEntity(id: 3, productId: activeProduct2.Id, quantity: 20));

//         await context.SaveChangesAsync();

//         var repository = CreateRepository(context);

//         // Act
//         var result = await repository.SelectAllAsync();

//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count);

//         Assert.Contains(result, product => product.Name == "エコバッグ");
//         Assert.Contains(result, product => product.Name == "耐水ノート(A5)");
//         Assert.DoesNotContain(result, product => product.Name == "高級ボールペン");

//         Log("SelectAllAsync：delete_flag = 0 の商品だけ取得できることを確認しました。");
//     }

//     /// <summary>
//     /// SelectByCategoryAsync で指定カテゴリの商品だけ取得できること
//     /// </summary>
//     [Fact(DisplayName = "SelectByCategoryAsyncで指定カテゴリの商品だけ取得できる")]
//     public async Task SelectByCategoryAsync_CategoryExists_ReturnsCategoryProducts()
//     {
//         // Arrange
//         Log("SelectByCategoryAsync_CategoryExists_ReturnsCategoryProducts：テスト開始");

//         using var context = CreateDbContext();

//         var categoryA = CreateCategoryEntity(
//             id: 1,
//             categoryUuid: "11111111-1111-1111-1111-111111111111",
//             name: "雑貨");

//         var categoryB = CreateCategoryEntity(
//             id: 2,
//             categoryUuid: "22222222-2222-2222-2222-222222222222",
//             name: "PC周辺機器");

//         var productA1 = CreateProductEntity(
//             id: 1,
//             productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
//             categoryId: categoryA.Id,
//             name: "エコバッグ",
//             price: 880,
//             imageUrl: "https://example.com/images/bag.jpg",
//             deleteFlag: 0,
//             category: categoryA);

//         var productA2 = CreateProductEntity(
//             id: 2,
//             productUuid: "9374cfe6-bc67-4147-92e6-9f8afab3c06b",
//             categoryId: categoryA.Id,
//             name: "耐水ノート(A5)",
//             price: 450,
//             imageUrl: "https://example.com/images/note.jpg",
//             deleteFlag: 0,
//             category: categoryA);

//         var productB1 = CreateProductEntity(
//             id: 3,
//             productUuid: "eb07baff-7f28-4356-abfb-020c31e04dc7",
//             categoryId: categoryB.Id,
//             name: "Type-C ハブ 6in1",
//             price: 3980,
//             imageUrl: "https://example.com/images/hub.jpg",
//             deleteFlag: 0,
//             category: categoryB);

//         await context.ProductCategories.AddRangeAsync(categoryA, categoryB);
//         await context.Products.AddRangeAsync(productA1, productA2, productB1);
//         await context.ProductStocks.AddRangeAsync(
//             CreateStockEntity(id: 1, productId: productA1.Id, quantity: 10),
//             CreateStockEntity(id: 2, productId: productA2.Id, quantity: 20),
//             CreateStockEntity(id: 3, productId: productB1.Id, quantity: 8));

//         await context.SaveChangesAsync();

//         var repository = CreateRepository(context);

//         // Act
//         var result = await repository.SelectByCategoryAsync(categoryA.CategoryUuid.ToString());

//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count);

//         Assert.Contains(result, product => product.Name == "エコバッグ");
//         Assert.Contains(result, product => product.Name == "耐水ノート(A5)");
//         Assert.DoesNotContain(result, product => product.Name == "Type-C ハブ 6in1");

//         Log("SelectByCategoryAsync：指定カテゴリの商品だけ取得できることを確認しました。");
//     }

//     /// <summary>
//     /// SelectByCategoryAsync で存在しないカテゴリUUIDを指定した場合、空の一覧が返ること
//     /// </summary>
//     [Fact(DisplayName = "SelectByCategoryAsyncで存在しないカテゴリUUIDの場合は空の一覧が返る")]
//     public async Task SelectByCategoryAsync_CategoryDoesNotExist_ReturnsEmptyList()
//     {
//         // Arrange
//         Log("SelectByCategoryAsync_CategoryDoesNotExist_ReturnsEmptyList：テスト開始");

//         using var context = CreateDbContext();

//         var category = CreateCategoryEntity(
//             id: 1,
//             categoryUuid: "11111111-1111-1111-1111-111111111111",
//             name: "雑貨");

//         var product = CreateProductEntity(
//             id: 1,
//             productUuid: "b7af7239-108b-4698-b2a7-2fe4469b275a",
//             categoryId: category.Id,
//             name: "エコバッグ",
//             price: 880,
//             imageUrl: "https://example.com/images/bag.jpg",
//             deleteFlag: 0,
//             category: category);

//         await context.ProductCategories.AddAsync(category);
//         await context.Products.AddAsync(product);
//         await context.ProductStocks.AddAsync(
//             CreateStockEntity(id: 1, productId: product.Id, quantity: 10));

//         await context.SaveChangesAsync();

//         var repository = CreateRepository(context);

//         var notExistCategoryUuid = "99999999-9999-9999-9999-999999999999";

//         // Act
//         var result = await repository.SelectByCategoryAsync(notExistCategoryUuid);

//         // Assert
//         Assert.NotNull(result);
//         Assert.Empty(result);

//         Log("SelectByCategoryAsync：存在しないカテゴリUUIDの場合、空の一覧が返ることを確認しました。");
//     }

//     /// <summary>
//     /// テスト用の AppDbContext を作成する
//     /// </summary>
//     /// <returns>AppDbContext</returns>
//     private static AppDbContext CreateDbContext()
//     {
//         var options = new DbContextOptionsBuilder<AppDbContext>()
//             .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
//             .Options;

//         return new AppDbContext(options);
//     }

//     /// <summary>
//     /// テスト用の ProductRepository を作成する
//     /// </summary>
//     /// <param name="context">AppDbContext</param>
//     /// <returns>ProductRepository</returns>
//     private static ProductRepository CreateRepository(AppDbContext context)
//     {
//         var factory = CreateFactory();

//         return new ProductRepository(
//             context,
//             factory,
//             productEntityAdapter: null!,
//             productStockAdapter: null!);
//     }

//     /// <summary>
//     /// テスト用の ProductFactory を作成する
//     /// </summary>
//     /// <returns>ProductFactory</returns>
//     private static ProductFactory CreateFactory()
//     {
//         var categoryRestorerMock =
//             new Mock<IRestorer<ProductCategory, ProductCategoryEntity>>();

//         var stockRestorerMock =
//             new Mock<IRestorer<ProductStock, ProductStockEntity>>();

//         categoryRestorerMock
//             .Setup(restorer => restorer.RestoreAsync(It.IsAny<ProductCategoryEntity>()))
//             .ReturnsAsync((ProductCategoryEntity entity) =>
//                 CreateProductCategory(
//                     entity.CategoryUuid.ToString(),
//                     entity.Name));

//         stockRestorerMock
//             .Setup(restorer => restorer.RestoreAsync(It.IsAny<ProductStockEntity>()))
//             .ReturnsAsync((ProductStockEntity entity) =>
//                 CreateProductStock(entity.Quantity));

//         return new ProductFactory(
//             categoryRestorerMock.Object,
//             stockRestorerMock.Object);
//     }

//     /// <summary>
//     /// テスト用の商品Entityを作成する
//     /// </summary>
//     private static ProductEntity CreateProductEntity(
//         int id,
//         string productUuid,
//         int categoryId,
//         string name,
//         int price,
//         string imageUrl,
//         int deleteFlag,
//         ProductCategoryEntity category)
//     {
//         return new ProductEntity
//         {
//             Id = id,
//             ProductUuid = Guid.Parse(productUuid),
//             ProductCategoryId = categoryId,
//             Name = name,
//             Price = price,
//             ImageUrl = imageUrl,
//             DeleteFlag = deleteFlag,
//             ProductCategory = category
//         };
//     }

//     /// <summary>
//     /// テスト用の商品在庫Entityを作成する
//     /// </summary>
//     private static ProductStockEntity CreateStockEntity(
//         int id,
//         int productId,
//         int quantity)
//     {
//         return new ProductStockEntity
//         {
//             Id = id,
//             ProductId = productId,
//             Quantity = quantity
//         };
//     }

//     /// <summary>
//     /// テスト用の商品カテゴリEntityを作成する
//     /// </summary>
//     private static ProductCategoryEntity CreateCategoryEntity(
//         int id,
//         string categoryUuid,
//         string name)
//     {
//         return new ProductCategoryEntity
//         {
//             Id = id,
//             CategoryUuid = Guid.Parse(categoryUuid),
    
//             Name = name
//         };
//     }

//     /// <summary>
//     /// テスト用のProductCategoryを作成する
//     /// </summary>
//     private static ProductCategory CreateProductCategory(
//         string categoryUuid,
//         string name)
//     {
//         var category =
//             (ProductCategory)FormatterServices.GetUninitializedObject(typeof(ProductCategory));

//         SetProperty(category, nameof(ProductCategory.CategoryUuid), categoryUuid);
//         SetProperty(category, nameof(ProductCategory.Name), name);

//         return category;
//     }

//     /// <summary>
//     /// テスト用のProductStockを作成する
//     /// </summary>
//     private static ProductStock CreateProductStock(int quantity)
//     {
//         var stock =
//             (ProductStock)FormatterServices.GetUninitializedObject(typeof(ProductStock));

//         SetProperty(stock, nameof(ProductStock.Quantity), quantity);

//         return stock;
//     }

//     /// <summary>
//     /// private set のプロパティにテスト用の値を設定する
//     /// </summary>
//     private static void SetProperty<T>(
//         T target,
//         string propertyName,
//         object value)
//     {
//         var property = typeof(T).GetProperty(
//             propertyName,
//             BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

//         if (property is null)
//         {
//             throw new InvalidOperationException(
//                 $"{typeof(T).Name} に {propertyName} プロパティが見つかりません。");
//         }

//         property.SetValue(target, value);
//     }
// }