using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品詳細取得APIを提供するController。
/// </summary>
[ApiController]
[Route("api/admin/products/info")]
public class GetProductInfoByIdController : ControllerBase
{
    private readonly IGetProductInfoUsecase _getProductInfoUsecase;
    private readonly GetProductViewModelAdapter _getProductViewModelAdapter;

    public GetProductInfoByIdController(
        IGetProductInfoUsecase getProductInfoUsecase,
        GetProductViewModelAdapter getProductViewModelAdapter)
    {
        _getProductInfoUsecase = getProductInfoUsecase;
        _getProductViewModelAdapter = getProductViewModelAdapter;
    }

    /// <summary>
    /// 商品UUIDに一致する商品詳細を取得する。
    /// </summary>
    /// <param name="productUuId">商品UUID。</param>
    /// <returns>商品詳細。</returns>
    [HttpGet]
    public async Task<ActionResult<GetProductInfoResponse>> GetInfoById(
        [FromQuery] string productUuId)
    {
        if (string.IsNullOrWhiteSpace(productUuId) ||
            !Guid.TryParse(productUuId, out _))
        {
            return BadRequest(new
            {
                message = "商品UUIDの形式が不正です。"
            });
        }

        var product = await _getProductInfoUsecase.ExecuteAsync(productUuId);

        if (product is null)
        {
            return NotFound(new
            {
                message = "商品が見つかりません。"
            });
        }

        var response = _getProductViewModelAdapter.Convert(product);

        return Ok(response);
    }
}