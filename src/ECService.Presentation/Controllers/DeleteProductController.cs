using ECService.Application.Usecases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品削除APIを提供するController
/// </summary>
[ApiController]
[Route("api/products")]
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
    public async Task<IActionResult> DeleteAsync(string productUuid)
    {
        await _deleteProductUsecase.ExecuteAsync(productUuid);

        return NoContent();
    }
}