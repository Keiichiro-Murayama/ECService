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
/// 担当者アカウント登録APIを提供するController
/// </summary>
//[Authorize]
[ApiController]
[Route("api/admin/accounts")]
[SwaggerTag("担当者アカウント登録API")]
public class RegisterEmployeeAccountController : ControllerBase
{
    private readonly IRegisterEmployeeAccountUsecase _usecase;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[アカウント名を登録する]を実現するインターフェイス</param>
    public RegisterEmployeeAccountController(
        IRegisterEmployeeAccountUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// アカウント名の登録
    /// </summary>
    /// <param name="request">ユースケース:[アカウント名を登録する]を実現するViewModel</param>
    /// <returns></returns>
    [HttpPost]
    [SwaggerOperation(Summary = "担当者アカウントを登録",
                      Description = "社員ID、アカウント名、パスワードを受け取り、担当者アカウントを登録する")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "未入力エラー")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "入力値エラー")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された社員IDが存在しない")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "アカウント名が既に存在する場合")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(EmployeeAccount))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]

    public async Task<IActionResult> Register(
        [FromBody, SwaggerRequestBody("担当者アカウント登録用Request", Required = true)]
        RegisterEmployeeAccountRequest request)
    {
        try
        {
            // DataAnnotationsのエラーは通常ここへ来る前に自動で400になる
            await _usecase.ExecuteAsync(
                request.EmployeeUuid,
                request.AccountName,
                request.Password);

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    message = "担当者アカウントを登録しました。"
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







