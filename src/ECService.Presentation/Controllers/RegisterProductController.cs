using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using DomainException = ECService.Domain.Exceptions.DomainException;
using InternalException = ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品登録APIを提供するController
/// </summary>
//[Authorize]
[ApiController]
[Route("api/admin/products")]
[SwaggerTag("商品登録API")]
public class RegisterProductController : ControllerBase
{
    private readonly IRegisterProductUsecase _usecase;
    private readonly RegisterProductViewModelAdapter _adapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品を登録する]を実現するインターフェイス</param>
    /// <param name="adapter">商品登録リクエストを商品ドメインへ変換するアダプタ</param>
    public RegisterProductController(
        IRegisterProductUsecase usecase,
        RegisterProductViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// 商品の登録
    /// </summary>
    /// <param name="request">ユースケース:[新商品を登録する]を実現するViewModel</param>
    /// <returns>登録結果</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "商品を登録",
        Description = "商品名、価格、在庫数、商品カテゴリUUID、画像URLを登録する")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "未入力エラー / 入力値エラー")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "重複エラー")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]


    public async Task<IActionResult> RegisterProduct(
        [FromBody] RegisterProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationError();
        }

        //石原:追加 空白だけの入力やnullable項目の未入力をControllerで判定する
        if (string.IsNullOrWhiteSpace(request.ProductName) ||
            request.Price is null ||
            request.Stock is null ||
            string.IsNullOrWhiteSpace(request.CategoryUuid))
        {
            return BadRequest(new
            {
                message = "入力値に不備があります。"
            });
        }

        //石原:追加 categoryUuidの形式チェックはAdapterではなくControllerで行う
        if (!Guid.TryParse(request.CategoryUuid, out _))
        {
            ModelState.AddModelError(
                nameof(request.CategoryUuid),
                "カテゴリUUIDの形式が不正です。");

            return ValidationError();
        }

        try
        {
            //石原:追加 Adapterへ渡す前に前後の空白を除去する
            request.ProductName = request.ProductName.Trim();
            request.CategoryUuid = request.CategoryUuid.Trim();

            Product product = await _adapter.RestoreAsync(request);

            await _usecase.ExecuteAsync(product);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    productUuid = product.ProductUuid,
                    name = product.Name,
                    message = "商品を登録しました。"
                });
        }
        catch (DomainException ex)
        {
            if (ex.Message.Contains("既に") ||
                ex.Message.Contains("重複"))
            {
                return Conflict(new
                {
                    message = "同じ商品名が既に登録されています。"
                });
            }

            return BadRequest(new
            {
                message = "入力値に不備があります。"
            });
        }
        catch (InternalException ex)
        {
            if (ex.Message.Contains("カテゴリ") ||
                ex.Message.Contains("UUID"))
            {
                return BadRequest(new
                {
                    message = "入力値に不備があります。"
                });
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }



    /// <summary>
    /// バリデーションエラー時のレスポンスを返す
    /// </summary>
    /// <returns></returns>
    private IActionResult ValidationError()
    {
        string message = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "入力値に不備があります。";

        return BadRequest(new
        {
            message
        });
    }
}
