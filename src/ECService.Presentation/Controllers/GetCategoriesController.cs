using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ECService.Domain.Models;
using ECService.Domain.Exceptions;
using ECService.Application.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace ECService.Presentation.Controllers;

/// <summary>
/// カテゴリ取得APIを提供するController
/// </summary>
[ApiController]
[Route("api/admin/categories")]
[SwaggerTag("カテゴリ取得API")]
[Authorize]
public class GetCategoriesController : ControllerBase
{
    private readonly IGetCategoriesUsecase _getCategoriesUsecase;
    private readonly GetCategoriesViewModelAdapter _getCategoriesViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="getCategoriesUsecase">商品検索ユースケース</param>
    /// <param name="getCategoriesViewModelAdapter">商品検索ViewModel変換アダプタ</param>
    public GetCategoriesController(
        IGetCategoriesUsecase getCategoriesUsecase,
        GetCategoriesViewModelAdapter getCategoriesViewModelAdapter)
    {
        _getCategoriesUsecase = getCategoriesUsecase;
        _getCategoriesViewModelAdapter = getCategoriesViewModelAdapter;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "カテゴリ一覧を取得",
        Description = "登録されているカテゴリ一覧を取得する")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "取得成功")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]

    public async Task<ActionResult<GetCategoriesResponse>> GetCategories()
    {
        // UseCaseからカテゴリ一覧を取得する
        var categories = await _getCategoriesUsecase.ExecuteAsync();

        // プルダウンに必要な項目だけ返す
        // var response = categories.Select(categories => new
        // {
        //     categoriUuid = categories.CategoryUuid,
        //     categoryName = categories.Name
        // });

        // GetCategoriesResponse response = new GetCategoriesResponse();


        var response = await _getCategoriesViewModelAdapter.Convert(categories);

        return Ok(response);
    }
}