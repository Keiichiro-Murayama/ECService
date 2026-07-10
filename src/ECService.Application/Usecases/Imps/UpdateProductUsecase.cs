using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

/// <summary>
/// 商品修正ユースケース。
/// </summary>
public class UpdateProductUsecase : IUpdateProductUsecase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public UpdateProductUsecase(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
    }

    /// <summary>
    /// 商品情報を修正する。
    /// </summary>
    /// <param name="productUuid">商品UUID。</param>
    /// <param name="name">商品名。</param>
    /// <param name="price">価格。</param>
    /// <param name="stock">在庫数。</param>
    /// <param name="categoryUuid">商品カテゴリUUID。</param>
    /// <param name="imageUrl">画像URL。</param>
    /// <returns>修正後の商品。</returns>
    public async Task<Product> ExecuteAsync(
        string productUuid,
        string name,
        int price,
        int stock,
        string categoryUuid,
        string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(productUuid) ||
            !Guid.TryParse(productUuid, out _))
        {
            throw new DomainException("商品UUIDの形式が不正です。", nameof(productUuid));
        }

        var product = await _productRepository.SelectByUuidAsync(productUuid);

        if (product is null)
        {
            throw new DomainException("商品が見つかりません。", nameof(productUuid));
        }

        var categories = await _productCategoryRepository.SelectAllAsync();

        var productCategory = categories
            .FirstOrDefault(category => category.CategoryUuid == categoryUuid);

        if (productCategory is null)
        {
            throw new DomainException("カテゴリを選択してください", nameof(categoryUuid));
        }

        product.ChangeName(name);
        product.ChangePrice(price);
        product.ChangeImageUrl(imageUrl);
        product.ChangeCategory(productCategory);
        product.ProductStock.ChangeQuantity(stock);

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var result = await _productRepository.UpdateAsync(product);

            if (!result)
            {
                throw new DomainException("商品情報を更新できませんでした。", nameof(productUuid));
            }

            await _unitOfWork.CommitAsync();

            return product;
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}