using ECService.Applications.Usecases.Interfaces;
using ECService.Applications.UseCases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
using ECService.Domains.Repositories;

namespace ECService.Applications.Usecases.Imps;

public class RegisterrProductUsecase : IRegisterProductUsecase
{

   private readonly IUnitOfWork _unitOfWork;
   private readonly IProductRepository _productRepository;

   private readonly IProductCategoryRepository _productCategoryRepository;


   public RegisterrProductUsecase(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IProductCategoryRepository productCategoryRepository)
    {
        _unitOfWork = unitOfWork; 
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
    }

        /// <summary>
    /// 指定ざれた商品の存在有無を調べる
    /// </summary>
    /// <param name="productName">商品目</param>
    /// <returns>なし</returns>
    /// <exception cref="ExistsException">同一商品名が存在する場合にスローされる</exception>
    public async Task ExistsByNameAsync(string name)
    {
        // 指定された商品の有無を調べる
        var result = await _productRepository.ExistsByNameAsync(name);
        if (result) // 商品が既に存在する
        {
            throw new DomainException($"商品名:{name}は既に存在します。");
        }
    }

        /// <summary>
    /// すべての商品カテゴリを取得する
    /// クライアント側の[入力画面]で利用するプルダウンを作成するため
    /// </summary>
    /// <returns>ProductCategoryのリスト</returns>
    public async Task<List<ProductCategory>> ExecuteAsync()
    {
        return await _productCategoryRepository.SelectAllAsync();
    }

        /// <summary>
    /// 新商品を登録する
    /// </summary>
    /// <param name="product">登録対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品カテゴリが存在しない場合にスローされる</exception>
    public async Task ExecuteAsync(Product product)
    {
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
    