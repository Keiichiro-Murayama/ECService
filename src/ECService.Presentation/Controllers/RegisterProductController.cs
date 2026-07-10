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







