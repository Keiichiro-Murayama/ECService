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
/// ユースケース:[アカウント名を登録する]を実現するコントローラ
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
    /// <param name="usecase">ユースケース:[アカウント名を登録する]を実現するインターフェイス</param>
    /// <param name="adapter">ユースケース:[アカウント名を登録する]を実現するインターフェイス</param>
    public RegisterProductController(
        IRegisterProductUsecase usecase,
        RegisterProductViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// アカウント名の登録
    /// </summary>
    /// <param name="request">ユースケース:[アカウント名を登録する]を実現するViewModel</param>
    /// <returns></returns>
    [HttpPost]
        [SwaggerOperation(Summary = "商品を登録",
                      Description = "商品ID、商品カテゴリ、商品名、在庫を登録する")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "未入力エラー")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "入力値エラー")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "アカウント名が既に存在する場合")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(EmployeeAccount))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]
    public async Task<IActionResult> Register(
        RegisterProductRequest request)
    {
        try
        {
            //RequestをProductに変換
            Product product = await _adapter.RestoreAsync(request);
            // DataAnnotationsのエラーは通常ここへ来る前に自動で400になる
            await _usecase.ExecuteAsync(
                product);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "新規商品を登録しました。"
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







