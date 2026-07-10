using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Exceptions;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品修正APIを提供するController。
/// </summary>
[ApiController]
[Route("api/admin/products/update")]
public class UpdateProductController : ControllerBase
{
    private readonly IUpdateProductUsecase _updateProductUsecase;

    public UpdateProductController(IUpdateProductUsecase updateProductUsecase)
    {
        _updateProductUsecase = updateProductUsecase;
    }

    /// <summary>
    /// 商品情報を修正する。
    /// </summary>
    /// <param name="productUuid">商品UUID。</param>
    /// <param name="model">商品修正リクエスト。</param>
    /// <returns>修正結果。</returns>
    [HttpPut]
    public async Task<IActionResult> UpdateProduct(
        [FromQuery] string productUuid,
        [FromBody] UpdateProductRequest model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _updateProductUsecase.ExecuteAsync(
                productUuid,
                model.ProductName,
                model.Price,
                model.Stock,
                model.CategoryId,
                model.ImageUrl
            );

            return Ok(new
            {
                message = "商品情報を更新しました。"
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}