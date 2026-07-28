using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ECService.Application.Usecases.Interfaces;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品画像のアップロード・削除API
/// </summary>
[ApiController]
[Authorize]
[Route("api/admin/product-images")]
[SwaggerTag("商品画像API")]
public sealed class ProductImageController
    : ControllerBase
{
    /// <summary>
    /// 商品画像の最大サイズ
    /// </summary>
    private const long MaxImageFileSize =
        2 * 1024 * 1024;

    /// <summary>
    /// アップロード可能な画像形式
    /// </summary>
    private static readonly string[]
        AllowedImageContentTypes =
        [
            "image/png",
            "image/jpeg",
        ];

    private readonly
        IProductImageStorage _productImageStorage;

    private readonly
        ILogger<ProductImageController> _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ProductImageController(
        IProductImageStorage productImageStorage,
        ILogger<ProductImageController> logger)
    {
        _productImageStorage =
            productImageStorage;

        _logger = logger;
    }

    /// <summary>
    /// 商品画像をアップロードする
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary =
            "商品画像をアップロードする")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "アップロード成功")]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "入力値エラー")]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "サーバーエラー")]
    public async Task<IActionResult> Upload(
        [FromForm]
        UploadProductImageRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationError();
        }

        if (
            string.IsNullOrWhiteSpace(
                request.ProductName) ||
            request.Image is null ||
            request.Image.Length == 0)
        {
            return BadRequest(new
            {
                message =
                    "入力値に不備があります。",
            });
        }

        if (
            request.Image.Length >
            MaxImageFileSize)
        {
            ModelState.AddModelError(
                nameof(request.Image),
                "画像は2MB以下にしてください。");

            return ValidationError();
        }

        if (
            !AllowedImageContentTypes.Contains(
                request.Image.ContentType,
                StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(
                nameof(request.Image),
                "PNG形式またはJPEG形式の画像を選択してください。");

            return ValidationError();
        }

        try
        {
            string productName =
                request.ProductName.Trim();

            await using Stream imageStream =
                request.Image.OpenReadStream();

            string imageUrl =
                await _productImageStorage
                    .UploadAsync(
                        imageStream,
                        productName,
                        request.Image.ContentType,
                        HttpContext
                            .RequestAborted);

            return Ok(new
            {
                imageUrl,
                message =
                    "商品画像をアップロードしました。",
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "商品画像のアップロードに失敗しました。");

            return StatusCode(
                StatusCodes
                    .Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: 商品画像のアップロードに失敗しました。",
                });
        }
    }

    /// <summary>
    /// 商品画像を削除する
    /// </summary>
    [HttpDelete]
    [SwaggerOperation(
        Summary =
            "商品画像を削除する")]
    [SwaggerResponse(
        StatusCodes.Status204NoContent,
        "削除成功")]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "入力値エラー")]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "サーバーエラー")]
    public async Task<IActionResult> Delete(
        [FromQuery]
        string imageUrl)
    {
        if (
            string.IsNullOrWhiteSpace(
                imageUrl))
        {
            return BadRequest(new
            {
                message =
                    "画像URLを指定してください。",
            });
        }

        try
        {
            await _productImageStorage
                .DeleteAsync(
                    imageUrl.Trim(),
                    HttpContext
                        .RequestAborted);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "商品画像の削除に失敗しました。ImageUrl: {ImageUrl}",
                imageUrl);

            return StatusCode(
                StatusCodes
                    .Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: 商品画像の削除に失敗しました。",
                });
        }
    }

    /// <summary>
    /// バリデーションエラーを返す
    /// </summary>
    private IActionResult ValidationError()
    {
        string message =
            ModelState.Values
                .SelectMany(
                    value => value.Errors)
                .Select(
                    error =>
                        error.ErrorMessage)
                .FirstOrDefault()
            ?? "入力値に不備があります。";

        return BadRequest(new
        {
            message,
        });
    }
}