using ECService.Application.Usecases.Interfaces;
using DomainException =
    ECService.Domain.Exceptions.DomainException;

using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 購入履歴検索APIを提供するController
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Tags("注文・購入履歴")]
public class SearchOrderHistoriesController : ControllerBase
{
    private readonly ISearchOrderHistoriesUsecase
        _searchOrderHistoriesUsecase;

    private readonly SearchOrderHistoriesViewModelAdapter
        _searchOrderHistoriesViewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SearchOrderHistoriesController(
        ISearchOrderHistoriesUsecase searchOrderHistoriesUsecase,
        SearchOrderHistoriesViewModelAdapter
            searchOrderHistoriesViewModelAdapter)
    {
        _searchOrderHistoriesUsecase =
            searchOrderHistoriesUsecase;

        _searchOrderHistoriesViewModelAdapter =
            searchOrderHistoriesViewModelAdapter;
    }

    /// <summary>
    /// 指定された条件で購入履歴を検索する
    /// </summary>
    /// <param name="request">購入履歴の検索条件</param>
    /// <returns>購入履歴一覧</returns>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(
        typeof(List<OrderHistoriesItem>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<OrderHistoriesItem>>> Search(
        [FromQuery] SearchOrderHistoriesRequest request)
    {
        try
        {
            var orders =
                await _searchOrderHistoriesUsecase.ExecuteAsync(
                    request.PurchaseDate,
                    request.CustomerAccountName
                );

            var response =
                _searchOrderHistoriesViewModelAdapter.Convert(orders);

            return Ok(response.OrderHistories);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InternalException ex)
        {
            Console.WriteLine(ex);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = ex.Message,
                    detail = ex.InnerException?.Message,
                    innerDetail = ex.InnerException?.InnerException?.Message
                }
            );
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "購入履歴検索中に予期しないエラーが発生しました。"
                }
            );
        }
    }
}