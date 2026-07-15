using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;

using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品検索APIを提供するController
/// </summary>
[ApiController]
//[Authorize]
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
        try
        {
            var products = await _searchProductsUsecase.ExecuteAsync(categoryUuid);

            var response = _searchProductsViewModelAdapter.Convert(products);

            return Ok(response.Products);
        }
        catch (DomainException)
        {
            return NotFound(new
            {
                message = "指定されたカテゴリID（UUID）が存在しません。"
            });
        }
        catch (InternalException ex)
        {
            if (ex.Message.Contains("カテゴリ") ||
                ex.Message.Contains("UUID"))
            {
                return NotFound(new
                {
                    message = "指定されたカテゴリID（UUID）が存在しません。"
                });
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }
}