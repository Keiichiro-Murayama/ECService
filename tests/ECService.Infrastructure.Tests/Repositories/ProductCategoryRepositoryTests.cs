using ECService.Domain.Models;
using ECService.Infrastructure.Adapters;
using ECService.Infrastructure.Contexts;
using ECService.Infrastructure.Entities;
using ECService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ECService.Infrastructure.Tests.Repositories;

/// <summary>
/// ProductCategoryRepositoryの単体テスト
/// </summary>
[TestClass]
public class ProductCategoryRepositoryTests
{
    private AppDbContext _context = null!;
    private ProductCategoryRepository _repository = null!;

    private readonly List<Guid> _categoryUuids = new();

    [TestInitialize]
    public async Task Setup()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettingsTests.json",
                optional: false,
                reloadOnChange: false)
                 .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration
            .GetConnectionString("ECServiceDB");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "appsettingsTests.json に ConnectionStrings:ECServiceDB が設定されていません。");
        }

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new AppDbContext(options);

        if (!await _context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException(
                "テスト用PostgreSQLへ接続できません。");
        }

        var adapter = new ProductCategoryEntityAdapter();

        _repository = new ProductCategoryRepository(
            _context,
            adapter);
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_context is null)
        {
            return;
        }

        _context.ChangeTracker.Clear();

        if (_categoryUuids.Count > 0)
        {
            var categories = await _context.ProductCategories
                .Where(category =>
                    _categoryUuids.Contains(category.CategoryUuid))
                .ToListAsync();

            _context.ProductCategories.RemoveRange(categories);

            await _context.SaveChangesAsync();
        }

        await _context.DisposeAsync();
    }

    [TestMethod(DisplayName = "UT-REP-CAT-001 商品カテゴリ一覧を取得できる")]
    public async Task SelectAllAsync_ShouldReturnProductCategories()
    {
        // Arrange
        var categoryName1 = CreateUniqueName("一覧カテゴリA");
        var categoryName2 = CreateUniqueName("一覧カテゴリB");

        var categoryEntity1 = await InsertCategoryAsync(categoryName1);
        var categoryEntity2 = await InsertCategoryAsync(categoryName2);

        // Act
        var result = await _repository.SelectAllAsync();

        // Assert
        var actualCategory1 = result.SingleOrDefault(category =>
            category.CategoryUuid == categoryEntity1.CategoryUuid.ToString());

        var actualCategory2 = result.SingleOrDefault(category =>
            category.CategoryUuid == categoryEntity2.CategoryUuid.ToString());

        Assert.IsNotNull(actualCategory1);
        Assert.AreEqual(categoryName1, actualCategory1.Name);

        Assert.IsNotNull(actualCategory2);
        Assert.AreEqual(categoryName2, actualCategory2.Name);
    }

    [TestMethod(DisplayName = "UT-REP-CAT-002 カテゴリ名が存在する場合はtrueを返す")]
    public async Task ExistsByNameAsync_ShouldReturnTrue_WhenCategoryNameExists()
    {
        // Arrange
        var categoryName = CreateUniqueName("存在確認カテゴリ");

        await InsertCategoryAsync(categoryName);

        // Act
        var result = await _repository.ExistsByNameAsync(categoryName);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UT-REP-CAT-003 カテゴリ名が存在しない場合はfalseを返す")]
    public async Task ExistsByNameAsync_ShouldReturnFalse_WhenCategoryNameDoesNotExist()
    {
        // Arrange
        var categoryName = CreateUniqueName("存在しないカテゴリ");

        // Act
        var result = await _repository.ExistsByNameAsync(categoryName);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "UT-REP-CAT-004 商品カテゴリを登録できる")]
    public async Task CreateAsync_ShouldCreateProductCategory()
    {
        // Arrange
        var categoryUuid = Guid.NewGuid();
        var categoryName = CreateUniqueName("登録カテゴリ");

        var productCategory = new ProductCategory(
            categoryUuid.ToString(),
            categoryName);

        _categoryUuids.Add(categoryUuid);

        // Act
        await _repository.CreateAsync(productCategory);

        // Assert
        var savedCategory = await _context.ProductCategories
            .AsNoTracking()
            .SingleOrDefaultAsync(category =>
                category.CategoryUuid == categoryUuid);

        Assert.IsNotNull(savedCategory);
        Assert.AreEqual(categoryName, savedCategory.Name);
    }

    private async Task<ProductCategoryEntity> InsertCategoryAsync(
        string name)
    {
        var category = new ProductCategoryEntity
        {
            CategoryUuid = Guid.NewGuid(),
            Name = name
        };

        _categoryUuids.Add(category.CategoryUuid);

        await _context.ProductCategories.AddAsync(category);
        await _context.SaveChangesAsync();

        return category;
    }

    private static string CreateUniqueName(
        string prefix)
    {
        return $"{prefix}_{Guid.NewGuid():N}"[..30];
    }
}