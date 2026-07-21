using ECService.Application.Exceptions;
using ECService.Application.Usecases.Interfaces;
using ECService.Domain.Models;
using ECService.Presentation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using DomainException = ECService.Domain.Exceptions.DomainException; //石原:追加 例外名の衝突を避ける
using InternalException = ECService.Infrastructure.Exceptions.InternalException; //石原:追加 共通500エラー用

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
    /// <returns>登録結果</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "担当者アカウントを登録",
        Description = "社員ID、アカウント名、パスワードを受け取り、担当者アカウントを登録する")]
    [SwaggerResponse(StatusCodes.Status201Created, "登録成功", typeof(EmployeeAccount))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "未入力エラー / 入力値エラー")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "指定された社員IDが存在しない")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "アカウント名または社員のアカウントが既に存在する場合")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]
    // [Authorize]

    public async Task<IActionResult> Register(
        [FromBody, SwaggerRequestBody("担当者アカウント登録用Request", Required = true)]
        RegisterEmployeeAccountRequest request)
    {
        //石原:追加 request自体がnull、または必須項目が空の場合はREADMEの未入力エラーを返す
        if (request is null ||
            string.IsNullOrWhiteSpace(request.EmployeeUuid) ||
            string.IsNullOrWhiteSpace(request.AccountName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "employeeId、accountName、passwordを入力してください。"
            });
        }

        //石原:追加 文字数などDataAnnotationsの入力値エラーはREADMEの入力値不備エラーを返す
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                message = "入力値に不備があります。"
            });
        }

        try
        {
            await _usecase.ExecuteAsync(
                request.EmployeeUuid.Trim(), //石原:追加 前後の空白を除去してUsecaseへ渡す
                request.AccountName.Trim(), //石原:追加 前後の空白を除去してUsecaseへ渡す
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
            //石原:追加 同じ社員UUIDで既にアカウント登録されている場合のエラーを分ける
            if (ex.Message.Contains("社員"))
            {
                return Conflict(new
                {
                    message = "この社員には既に担当者アカウントが登録されています。"
                });
            }

            return Conflict(new
            {
                message = "このアカウント名は既に登録されています。"
            });
        }
        catch (ExistsEmployeeException)
        {
            return NotFound(new
            {
                message = "指定された社員IDが存在しません。"
            });
        }
        catch (DomainException)
        {
            return BadRequest(new
            {
                message = "入力値に不備があります。"
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
}