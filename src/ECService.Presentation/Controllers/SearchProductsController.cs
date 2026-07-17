using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品検索APIを提供するController
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
    private readonly SearchProductsViewModelAdapter
        _searchProductsViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="searchProductsUsecase">
    /// 商品検索ユースケース
    /// </param>
    /// <param name="searchProductsViewModelAdapter">
    /// 商品検索レスポンス変換アダプタ
    /// </param>
    public SearchProductsController(
        ISearchProductsUsecase searchProductsUsecase,
        SearchProductsViewModelAdapter searchProductsViewModelAdapter)
    {
        _searchProductsUsecase = searchProductsUsecase;
        _searchProductsViewModelAdapter =
            searchProductsViewModelAdapter;
    }

    /// <summary>
    /// 商品を検索する
    /// </summary>
    /// <param name="categoryUuid">
    /// 商品カテゴリUUID。指定されない場合は全商品を検索する。
    /// </param>
    /// <returns>商品一覧</returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(List<ProductsItem>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductsItem>>> Search(
        [FromQuery] string? categoryUuid)
    {
        // カテゴリUUIDが指定されており、
        // UUID形式でない場合は400 Bad Requestを返す
        if (!string.IsNullOrWhiteSpace(categoryUuid)
            && !Guid.TryParse(categoryUuid, out _))
        {
            return BadRequest(new
            {
                message = "カテゴリUUIDの形式が正しくありません。"
            });
        }

        try
        {
            // 商品検索ユースケースを実行する
            var products =
                await _searchProductsUsecase.ExecuteAsync(categoryUuid);

            // ドメインオブジェクトをレスポンス形式へ変換する
            var response =
                _searchProductsViewModelAdapter.Convert(products);

            return Ok(response.Products);
        }
        catch (DomainException)
        {
            // UUID形式は正しいが、指定されたカテゴリが存在しない場合
            return NotFound(new
            {
                message =
                    "指定されたカテゴリID（UUID）が存在しません。"
            });
        }
        catch (InternalException ex)
        {
            // 現在の既存仕様に合わせ、カテゴリ・UUIDに関する
            // InternalExceptionは404として返す
            if (ex.Message.Contains("カテゴリ")
                || ex.Message.Contains("UUID"))
            {
                return NotFound(new
                {
                    message =
                        "指定されたカテゴリID（UUID）が存在しません。"
                });
            }

            // DB接続エラーなどの内部エラーの場合
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
        catch (Exception)
        {
            // その他の予期しないエラーの場合
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }
}
