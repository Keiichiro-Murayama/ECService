using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;

namespace ECService.Application.Usecases.Imps;

public class RegisterProductCategoryUsecase : IRegisterProductCategoryUsecase
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductCategoryRepository _productCategoryRepository;

    public RegisterProductCategoryUsecase(
         IUnitOfWork unitOfWork,
         IProductCategoryRepository productCategoryRepository)
    {
        _unitOfWork = unitOfWork;
        _productCategoryRepository = productCategoryRepository;
    }


    /// <summary>
    /// 新商品を登録する
    /// </summary>
    /// <param name="product">登録対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品カテゴリが存在しない場合にスローされる</exception>
    public async Task ExecuteAsync(ProductCategory productCategory)
    {
        // カテゴリ全件取得
        await _productCategoryRepository.SelectAllAsync();

        // 指定された商品カテゴリの有無を調べる
        var result = await _productCategoryRepository.ExistsByNameAsync(productCategory.Name);
        if (result) // 商品が既に存在する
        {
            throw new DomainException($"カテゴリ名:{productCategory.Name}は既に存在します。");
        }
        // トランザクションを開始する
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // カテゴリを新規登録する
            await _productCategoryRepository.CreateAsync(productCategory);
            // トランザクションをコミットする
            await _unitOfWork.CommitAsync();
        }
        catch
        {
            // トランザクションをロールバックする
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }


}
