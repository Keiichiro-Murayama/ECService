using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

using DomainException = ECService.Domain.Exceptions.DomainException;

namespace ECService.Application.Usecases.Imps;

public class SearchProductsUsecase : ISearchProductsUsecase
{
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public SearchProductsUsecase(
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
    }

    public async Task<List<Product>> ExecuteAsync(string? categoryUuid)
    {
        if (string.IsNullOrWhiteSpace(categoryUuid))
        {
            return await _productRepository.SelectAllAsync();
        }

        if (!Guid.TryParse(categoryUuid, out _))
        {
            throw new DomainException(
                "指定されたカテゴリID（UUID）が存在しません。",
                nameof(categoryUuid));
        }

        var categories = await _productCategoryRepository.SelectAllAsync();

        var existsCategory = categories
            .Any(category => category.CategoryUuid == categoryUuid);

        if (!existsCategory)
        {
            throw new DomainException(
                "指定されたカテゴリID（UUID）が存在しません。",
                nameof(categoryUuid));
        }

        return await _productRepository.SelectByCategoryAsync(categoryUuid);
    }
}