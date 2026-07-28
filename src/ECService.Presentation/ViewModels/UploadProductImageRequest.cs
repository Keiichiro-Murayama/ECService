using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace ECService.Presentation.ViewModels;

/// <summary>
/// 商品画像アップロードリクエスト
/// </summary>
public sealed class UploadProductImageRequest
{
    /// <summary>
    /// 商品名
    /// </summary>
    [Required(
        ErrorMessage =
            "商品名を入力してください。")]
    [StringLength(
        20,
        MinimumLength = 2,
        ErrorMessage =
            "商品名は2～20文字で入力してください。")]
    public string ProductName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// アップロードする商品画像
    /// </summary>
    [Required(
        ErrorMessage =
            "商品画像を選択してください。")]
    public IFormFile? Image
    {
        get;
        set;
    }
}