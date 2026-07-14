using ECService.Application.Usecases.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品削除APIを提供するController
/// </summary>
[ApiController]
[Authorize]
[Route("api/admin/products")]
public class DeleteProductController : ControllerBase
{
    private readonly IDeleteProductUsecase _deleteProductUsecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="deleteProductUsecase">
    /// 商品削除ユースケース
    /// </param>
    public DeleteProductController(
        IDeleteProductUsecase deleteProductUsecase)
    {
        _deleteProductUsecase = deleteProductUsecase;
    }

    /// <summary>
    /// 指定された商品を削除する
    /// </summary>
    /// <param name="productUuid">削除対象の商品UUID</param>
    /// <returns>削除結果</returns>
    [HttpDelete("{productUuid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(string productUuid)
    {
        if (!Guid.TryParse(productUuid, out _))
        {
            return BadRequest(new
            {
                message = "商品UUIDの形式が正しくありません。"
            });
        }

        try
        {
            await _deleteProductUsecase.ExecuteAsync(productUuid);

            return Ok(new
            {
                message = "商品を削除しました。"
            });
        }
        catch (InvalidOperationException)
        {
            return NotFound(new
            {
                message = "指定された商品が見つかりません。"
            });
        }
    }
}