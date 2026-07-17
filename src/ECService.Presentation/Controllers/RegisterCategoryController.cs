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
/// カテゴリ登録APIを提供するController
/// </summary>
//[Authorize]
[ApiController]
[Route("api/admin/categories")]
[SwaggerTag("カテゴリ登録API")]
public class RegisterCategoryController : ControllerBase
{
    private readonly IRegisterProductCategoryUsecase _usecase;
    private readonly RegisterCategoryViewModelAdapter _adapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[カテゴリを登録する]を実現するインターフェイス</param>
    /// <param name="adapter">カテゴリ登録リクエストをカテゴリドメインへ変換するアダプタ</param>
    public RegisterCategoryController(
        IRegisterProductCategoryUsecase usecase,
        RegisterCategoryViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// カテゴリの登録
    /// </summary>
    /// <param name="request">ユースケース:[カテゴリを登録する]を実現するViewModel</param>
    /// <returns>登録結果</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "カテゴリ登録",
        Description = "カテゴリを登録する")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "入力値エラー / 未入力エラー")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "カテゴリ名が既に存在する場合")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]
    [Authorize]

    public async Task<IActionResult> Register(
        [FromBody] RegisterCategoryRequest request)
    {
        if (request is null ||
            !ModelState.IsValid ||
            string.IsNullOrWhiteSpace(request.CategoryName))
        {
            return BadRequest(new
            {
                message = "カテゴリ名を入力してください。"
            });
        }

        try
        {
            request.CategoryName = request.CategoryName.Trim();

            ProductCategory category = await _adapter.RestoreAsync(request);

            await _usecase.ExecuteAsync(category);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    categoryUuid = category.CategoryUuid,
                    message = "商品カテゴリを登録しました。"
                });
        }
        catch (DomainException ex)
        {
            if (ex.Message.Contains("既に") ||
                ex.Message.Contains("重複"))
            {
                return Conflict(new
                {
                    message = "このカテゴリ名は既に登録されています。"
                });
            }

            return BadRequest(new
            {
                message = "カテゴリ名を入力してください。"
            });
        }
        catch (InternalException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("既に") ||
                ex.Message.Contains("重複"))
            {
                return Conflict(new
                {
                    message = "このカテゴリ名は既に登録されています。"
                });
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = "InternalException: サーバー内部で予期せぬエラーが発生しました。"
                });
        }
    }
}