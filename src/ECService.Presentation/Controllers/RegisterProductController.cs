using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;

using Microsoft.AspNetCore.Mvc;

using Swashbuckle.AspNetCore.Annotations;

using DomainException =
    ECService.Domain.Exceptions.DomainException;

using InternalException =
    ECService.Infrastructure.Exceptions.InternalException;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 商品登録APIを提供するController
/// </summary>
//[Authorize]
[ApiController]
[Route("api/admin/products")]
[SwaggerTag("商品登録API")]
public class RegisterProductController
    : ControllerBase
{
    private const long MaxImageFileSize =
        2 * 1024 * 1024;

    private static readonly string[]
        AllowedImageContentTypes =
        [
            "image/png",
            "image/jpeg",
        ];

    private readonly
        IRegisterProductUsecase _usecase;

    private readonly
        RegisterProductViewModelAdapter _adapter;

    //石原:追加 商品画像をAzure Blob Storageへ保存する処理
    private readonly
        IProductImageStorage _productImageStorage;

    //石原:追加 エラー内容を記録するLogger
    private readonly
        ILogger<RegisterProductController> _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RegisterProductController(
        IRegisterProductUsecase usecase,
        RegisterProductViewModelAdapter adapter,
        //石原:追加 Blob保存処理をDIから受け取る
        IProductImageStorage productImageStorage,
        //石原:追加 LoggerをDIから受け取る
        ILogger<RegisterProductController> logger)
    {
        _usecase = usecase;
        _adapter = adapter;

        //石原:追加 Blob保存処理を保持する
        _productImageStorage =
            productImageStorage;

        //石原:追加 Loggerを保持する
        _logger = logger;
    }

    /// <summary>
    /// 商品を登録する
    /// </summary>
    /// <param name="request">
    /// 商品登録リクエスト
    /// </param>
    /// <returns>
    /// 商品登録結果
    /// </returns>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "商品を登録",
        Description =
            "商品名、価格、在庫数、商品カテゴリUUID、商品画像を登録する")]
    [SwaggerResponse(
        StatusCodes.Status201Created,
        "登録成功")]
    [SwaggerResponse(
        StatusCodes.Status400BadRequest,
        "未入力エラー / 入力値エラー")]
    [SwaggerResponse(
        StatusCodes.Status409Conflict,
        "重複エラー")]
    [SwaggerResponse(
        StatusCodes.Status500InternalServerError,
        "予期せぬサーバーエラー")]
    public async Task<IActionResult>
        RegisterProduct(
            //石原:変更 商品情報と画像をフォームデータから受け取る
            [FromForm]
            RegisterProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationError();
        }

        //石原:変更 商品画像を含む必須項目を確認する
        if (
            string.IsNullOrWhiteSpace(
                request.ProductName) ||
            request.Price is null ||
            request.Stock is null ||
            string.IsNullOrWhiteSpace(
                request.CategoryUuid) ||
            request.Image is null ||
            request.Image.Length == 0)
        {
            return BadRequest(new
            {
                message =
                    "入力値に不備があります。",
            });
        }

        //石原:追加 商品画像のファイルサイズを確認する
        if (
            request.Image.Length >
            MaxImageFileSize)
        {
            ModelState.AddModelError(
                nameof(request.Image),
                "画像は2MB以下にしてください。");

            return ValidationError();
        }

        //石原:追加 PNGまたはJPEG形式か確認する
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

        //石原:追加 カテゴリUUIDの形式を確認する
        if (
            !Guid.TryParse(
                request.CategoryUuid,
                out _))
        {
            ModelState.AddModelError(
                nameof(request.CategoryUuid),
                "カテゴリUUIDの形式が不正です。");

            return ValidationError();
        }

        //石原:追加 DB登録失敗時に画像を削除できるようURLを保持する
        string? uploadedImageUrl = null;

        try
        {
            //石原:追加 前後の空白を除去してから処理する
            request.ProductName =
                request.ProductName.Trim();

            request.CategoryUuid =
                request.CategoryUuid.Trim();

            //石原:追加 受け取った商品画像のストリームを開く
            await using Stream imageStream =
                request.Image.OpenReadStream();

            //石原:変更 商品名を含むBlob名で画像を保存する
            string imageUrl =
                await _productImageStorage
                    .UploadAsync(
                        imageStream,
                        request.ProductName,
                        request.Image.ContentType,
                        HttpContext
                            .RequestAborted);

            uploadedImageUrl = imageUrl;

            //石原:変更 BlobのURLをAdapterへ別引数で渡す
            Product product =
                await _adapter.RestoreAsync(
                    request,
                    imageUrl);

            await _usecase.ExecuteAsync(
                product);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    productUuid =
                        product.ProductUuid,

                    name =
                        product.Name,

                    imageUrl,

                    message =
                        "商品を登録しました。",
                });
        }
        catch (DomainException ex)
        {
            //石原:追加 商品登録失敗時に保存済み画像を削除する
            await DeleteUploadedImageSafelyAsync(
                uploadedImageUrl);

            if (
                ex.Message.Contains("既に") ||
                ex.Message.Contains("重複"))
            {
                return Conflict(new
                {
                    message =
                        "同じ商品名が既に登録されています。",
                });
            }

            return BadRequest(new
            {
                message =
                    "入力値に不備があります。",
            });
        }
        catch (InternalException ex)
        {
            //石原:追加 商品登録失敗時に保存済み画像を削除する
            await DeleteUploadedImageSafelyAsync(
                uploadedImageUrl);

            if (
                ex.Message.Contains(
                    "カテゴリ") ||
                ex.Message.Contains(
                    "UUID"))
            {
                return BadRequest(new
                {
                    message =
                        "入力値に不備があります。",
                });
            }

            return StatusCode(
                StatusCodes
                    .Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: サーバー内部で予期せぬエラーが発生しました。",
                });
        }
        catch (Exception ex)
        {
            //石原:追加 予期せぬ失敗時にも保存済み画像を削除する
            await DeleteUploadedImageSafelyAsync(
                uploadedImageUrl);

            //石原:追加 想定外の例外をログへ記録する
            _logger.LogError(
                ex,
                "商品登録処理で予期せぬエラーが発生しました。");

            return StatusCode(
                StatusCodes
                    .Status500InternalServerError,
                new
                {
                    message =
                        "InternalException: サーバー内部で予期せぬエラーが発生しました。",
                });
        }
    }

    /// <summary>
    /// Blobへ保存した画像を安全に削除する
    /// </summary>
    /// <param name="imageUrl">
    /// 削除する画像URL
    /// </param>
    private async Task
        DeleteUploadedImageSafelyAsync(
            string? imageUrl)
    {
        if (
            string.IsNullOrWhiteSpace(
                imageUrl))
        {
            return;
        }

        try
        {
            //石原:追加 商品登録に失敗した場合は不要な画像を削除する
            await _productImageStorage
                .DeleteAsync(
                    imageUrl,
                    CancellationToken.None);
        }
        catch (Exception ex)
        {
            //石原:追加 画像削除の失敗はログへ記録する
            _logger.LogError(
                ex,
                "商品登録失敗後の画像削除に失敗しました。ImageUrl: {ImageUrl}",
                imageUrl);
        }
    }

    /// <summary>
    /// バリデーションエラーを返す
    /// </summary>
    /// <returns>
    /// 400レスポンス
    /// </returns>
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