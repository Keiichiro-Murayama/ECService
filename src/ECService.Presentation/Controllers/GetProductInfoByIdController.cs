using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品詳細取得APIを提供するController
/// </summary>
[ApiController]
[Route("api/admin/products/info")]
public class GetProductInfoByIdController : ControllerBase
{
    private readonly IGetProductInfoUsecase _getProductInfoUsecase;
    private readonly GetProductViewModelAdapter _adapter;

    public GetProductInfoByIdController(
        IGetProductInfoUsecase getProductInfoUsecase,
        GetProductViewModelAdapter adapter)
    {
        _getProductInfoUsecase = getProductInfoUsecase;
        _adapter = adapter;
    }

    [HttpGet]
    [Authorize]

    public async Task<ActionResult<GetProductInfoResponse>> GetInfoById(
        [FromQuery] string productUuId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productUuId) ||
                !Guid.TryParse(productUuId, out _))
            {
                throw new DomainException(
                    "商品UUIDの形式が不正です。",
                    nameof(productUuId));
            }

            var product = await _getProductInfoUsecase.ExecuteAsync(productUuId);

            if (product is null)
            {
                return NotFound(new { message = "指定された商品が見つかりません。" });
            }

            var response = _adapter.Convert(product);

            return Ok(response);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}