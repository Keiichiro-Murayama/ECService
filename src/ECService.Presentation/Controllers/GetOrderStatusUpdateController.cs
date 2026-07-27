using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using DomainException =
    ECService.Domain.Exceptions.DomainException;

using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 注文ステータス更新画面の表示情報を取得するAPI。
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Tags("注文・購入履歴")]

public class GetOrderStatusUpdateController : ControllerBase
{
    private readonly IGetOrderStatusUpdateUsecase
        _getOrderStatusUpdateUsecase;

    private readonly GetOrderStatusUpdateViewModelAdapter
        _getOrderStatusUpdateViewModelAdapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public GetOrderStatusUpdateController(
        IGetOrderStatusUpdateUsecase getOrderStatusUpdateUsecase,
        GetOrderStatusUpdateViewModelAdapter
            getOrderStatusUpdateViewModelAdapter)
    {
        _getOrderStatusUpdateUsecase =
            getOrderStatusUpdateUsecase;

        _getOrderStatusUpdateViewModelAdapter =
            getOrderStatusUpdateViewModelAdapter;
    }

    /// <summary>
    /// 注文ステータス更新画面の表示情報を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <returns>
    /// 対象注文の情報と、選択可能な注文ステータス一覧。
    /// </returns>
    [HttpGet("{orderUuid}/status")]
    [Authorize]
    [ProducesResponseType(
        typeof(GetOrderStatusUpdateResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetOrderStatusUpdateResponse>> Get(
        [FromRoute] string orderUuid)
    {
        if (string.IsNullOrWhiteSpace(orderUuid))
        {
            return BadRequest(new
            {
                message = "注文UUIDは必須です。"
            });
        }

        if (!Guid.TryParse(orderUuid, out _))
        {
            return BadRequest(new
            {
                message = "注文UUIDの形式が不正です。"
            });
        }

        try
        {
            var result =
                await _getOrderStatusUpdateUsecase.ExecuteAsync(
                    orderUuid
                );

            var response =
                _getOrderStatusUpdateViewModelAdapter.Convert(
                    result.Order,
                    result.OrderStatuses
                );

            return Ok(response);
        }
        catch (DomainException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InternalException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message =
                        "注文情報取得中にサーバー内部でエラーが発生しました。"
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
                        "注文情報取得中に予期しないエラーが発生しました。"
                }
            );
        }
    }
}