using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
// using ECService.Domains.Repositories;

namespace ECService.Application.Usecases.Imps;

public class RegisterProductUsecase : IRegisterProductUsecase
{

   private readonly IUnitOfWork _unitOfWork;
   private readonly IProductRepository _productRepository;

   public RegisterProductUsecase(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork; 
        _productRepository = productRepository;
    }

 
           /// <summary>
    /// 新商品を登録する
    /// </summary>
    /// <param name="product">登録対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品カテゴリが存在しない場合にスローされる</exception>
    public async Task ExecuteAsync(Product product)
    {
        // 指定された商品の有無を調べる
        var result = await _productRepository.ExistsByNameAsync(product.Name);
        if (result) // 商品が既に存在する
        {
            throw new DomainException($"商品名:{product.Name}は既に存在します。");
        }
        // トランザクションを開始する
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 新商品を登録する
            await _productRepository.CreateAsync(product);
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
    