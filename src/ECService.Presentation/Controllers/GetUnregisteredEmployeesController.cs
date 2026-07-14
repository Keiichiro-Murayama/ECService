using ECService.Application.Usecases.Imps;
using ECService.Application.Usecases.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using ECService.Presentation.Adapters;

namespace ECService.Presentation.Controllers;

/// <summary>
/// 社員情報を扱うController
/// </summary>
//[Authorize]
[ApiController]
[Route("api/admin/employees/unregisted")]
[SwaggerTag("未登録社員取得API")]
public class GetUnregisteredEmployeesController : ControllerBase
{
    private readonly IGetUnregisteredEmployeesUsecase _usecase;
    private readonly UnregisteredEmployeesViewModelAdapter _adapter;

    public GetUnregisteredEmployeesController(
        IGetUnregisteredEmployeesUsecase usecase,
        UnregisteredEmployeesViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// アカウント未登録の社員一覧を取得する
    /// </summary>
    [HttpGet("unregistered")]
    [SwaggerOperation(
        Summary = "未登録社員一覧を取得",
        Description = "担当者アカウントが未登録の社員一覧を取得する")]
    [SwaggerResponse(
        StatusCodes.Status200OK,
        "取得成功")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "予期せぬサーバーエラー")]

    public async Task<IActionResult> GetUnregisteredEmployees()
    {
        // UseCaseから未登録社員一覧を取得する
        var employees = await _usecase.ExecuteAsync();

        // プルダウンに必要な項目だけ返す
        var response = await _adapter.Convert(employees);

        return Ok(response);
    }
}