using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品検索機能のController
/// </summary>
[ApiController]
[Route("admin/product/search")]
public class SearchProductsController : ControllerBase
{
    private readonly ISearchProductsUseCase _searchProductsUseCase;
    private readonly SearchProductsViewModelAdapter _searchProductsViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="searchProductsUseCase">商品検索ユースケース</param>
    /// <param name="searchProductsViewModelAdapter">商品検索ViewModel変換アダプタ</param>
    public SearchProductsController(
        ISearchProductsUseCase searchProductsUseCase,
        SearchProductsViewModelAdapter searchProductsViewModelAdapter)
    {
        _searchProductsUseCase = searchProductsUseCase;
        _searchProductsViewModelAdapter = searchProductsViewModelAdapter;
    }

    /// <summary>
    /// 商品を検索する
    /// </summary>
    /// <param name="categoryUuid">商品カテゴリUUID</param>
    /// <returns>商品検索結果</returns>
    [HttpGet]
    public async Task<ActionResult<SearchProductsResponse>> Search(
        [FromQuery] string? categoryUuid)
    {
        var products = await _searchProductsUseCase.ExecuteAsync(categoryUuid);

        var response = _searchProductsViewModelAdapter.Convert(products);

        return Ok(response);
    }
}