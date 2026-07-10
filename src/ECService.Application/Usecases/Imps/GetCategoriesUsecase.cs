using ECService.Application.Usecases.Interfaces;
using ECService.Application.Usecases.UnitOfWorks;
using ECService.Domain.Exceptions;
using ECService.Domain.Models;
using ECService.Domain.Repositories;
// using ECService.Domains.Repositories;

namespace ECService.Applications.Usecases.Imps;

public class GetCategoriesUsecase : IGetCategoriesUsecase
{

   private readonly IUnitOfWork _unitOfWork;
   private readonly IProductCategoryRepository _productCategoryRepository;

   public GetCategoriesUsecase(
        IUnitOfWork unitOfWork,
        IProductCategoryRepository productCategoryRepository)
    {
        _unitOfWork = unitOfWork; 
        _productCategoryRepository = productCategoryRepository;
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


}
    