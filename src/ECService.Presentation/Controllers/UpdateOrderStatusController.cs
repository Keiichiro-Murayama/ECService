using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using DomainException =
    ECService.Domain.Exceptions.DomainException;

using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 注文ステータス更新API。
/// </summary>
[ApiController]
[Route("api/admin/orders")]
[Tags("注文・購入履歴")]
public class UpdateOrderStatusController : ControllerBase
{
    private readonly IUpdateOrderStatusUsecase
        _updateOrderStatusUsecase;

    private readonly UpdateOrderStatusViewModelAdapter
        _updateOrderStatusViewModelAdapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public UpdateOrderStatusController(
        IUpdateOrderStatusUsecase updateOrderStatusUsecase,
        UpdateOrderStatusViewModelAdapter
            updateOrderStatusViewModelAdapter)
    {
        _updateOrderStatusUsecase =
            updateOrderStatusUsecase;

        _updateOrderStatusViewModelAdapter =
            updateOrderStatusViewModelAdapter;
    }

    /// <summary>
    /// 指定された注文のステータスを更新する。
    /// </summary>
    /// <param name="orderUuid">注文UUID。</param>
    /// <param name="request">変更後の注文ステータス。</param>
    /// <returns>注文ステータス更新結果。</returns>
    [HttpPut("{orderUuid}/status")]
    [Authorize]
    [ProducesResponseType(
        typeof(UpdateOrderStatusResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UpdateOrderStatusResponse>> Update(
        [FromRoute] string orderUuid,
        [FromBody] UpdateOrderStatusRequest request)
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
                await _updateOrderStatusUsecase.ExecuteAsync(
                    orderUuid,
                    request.OrderStatusId
                );

            var response =
                _updateOrderStatusViewModelAdapter.Convert(
                    result.Order,
                    result.UpdatedAt
                );

            return Ok(response);
        }
        catch (DomainException ex)
        {
            return BadRequest(new
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
                        "注文ステータス更新中にサーバー内部でエラーが発生しました。"
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
                        "注文ステータス更新中に予期しないエラーが発生しました。"
                }
            );
        }
    }
}