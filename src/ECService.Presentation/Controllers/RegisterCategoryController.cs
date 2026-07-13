using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ECService.Domain.Models;
using ECService.Application.Usecases.Interfaces;
using ECService.Presentation.Adapters;
using ECService.Presentation.ViewModels;
using ECService.Domain.Exceptions;
using ECService.Application.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;


namespace ECService.Presentation.Controllers;
/// <summary>
/// ユースケース:[カテゴリを登録する]を実現するコントローラ
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
    /// <param name="adapter">ユースケース:変換を実現するインターフェイス</param>
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
    /// <returns></returns>
    [HttpPost]
        [SwaggerOperation(Summary = "カテゴリ登録",
                      Description = "カテゴリを登録する")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "未入力エラー")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "入力値エラー")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "カテゴリ名が既に存在する場合")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(EmployeeAccount))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]
    public async Task<IActionResult> Register(
        RegisterCategoryRequest request)
    {
        try
        {
            //RequestをProductに変換
            ProductCategory category = await _adapter.RestoreAsync(request);
            // DataAnnotationsのエラーは通常ここへ来る前に自動で400になる
            await _usecase.ExecuteAsync(
                category);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "商品カテゴリを登録しました。"
                });
        }
        catch (ExistsAccountException ex)
        {
            return Conflict(new
            {
                code = "ACCOUNT_ALREADY_EXISTS",
                message = ex.Message
            });
        }
        catch (ExistsEmployeeException ex)
        {
            return NotFound(new
            {
                code = "EMPLOYEE_NOT_FOUND",
                message = ex.Message
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                code = "DOMAIN_RULE_VIOLATION",
                message = ex.Message
            });
        }
    }
}