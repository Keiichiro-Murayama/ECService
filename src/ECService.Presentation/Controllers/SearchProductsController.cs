using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品検索APIコントローラー
/// </summary>
[ApiController]
[Authorize]
[Route("api/admin/products")]
public class SearchProductsController : ControllerBase
{
    /// <summary>
    /// 商品検索ユースケース
    /// </summary>
    private readonly ISearchProductsUsecase _searchProductsUsecase;

    /// <summary>
    /// 商品検索レスポンス変換アダプタ
    /// </summary>
    private readonly SearchProductsViewModelAdapter _searchProductsViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="searchProductsUsecase">商品検索ユースケース</param>
    /// <param name="searchProductsViewModelAdapter">商品検索レスポンス変換アダプタ</param>
    public SearchProductsController(
        ISearchProductsUsecase searchProductsUsecase,
        SearchProductsViewModelAdapter searchProductsViewModelAdapter)
    {
        _searchProductsUsecase = searchProductsUsecase;
        _searchProductsViewModelAdapter = searchProductsViewModelAdapter;
    }

    /// <summary>
    /// 商品を検索する
    /// </summary>
    /// <param name="categoryUuid">
    /// 商品カテゴリUUID。
    /// 指定されない場合は全商品を検索する。
    /// </param>
    /// <returns>商品一覧</returns>
    [HttpGet]
    public async Task<ActionResult<List<ProductsItem>>> Search(
        [FromQuery] string? categoryUuid)
    {
        // 商品検索ユースケースを実行する
        var products = await _searchProductsUsecase.ExecuteAsync(categoryUuid);

        // DomainのProduct一覧をレスポンス用ViewModelへ変換する
        var response = _searchProductsViewModelAdapter.Convert(products);

        // READMEに合わせて、productsプロパティではなく商品一覧の配列を返す
        return Ok(response.Products);
    }
}