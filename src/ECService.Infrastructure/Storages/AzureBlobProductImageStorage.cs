using System.Text;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using ECService.Application.Usecases.Interfaces;

namespace ECService.Infrastructure.Storages;

/// <summary>
/// Azure Blob Storageを使用して商品画像を保存する
/// </summary>
public sealed class AzureBlobProductImageStorage
    : IProductImageStorage
{
    private readonly
        BlobContainerClient _containerClient;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="containerClient">
    /// 商品画像用Blobコンテナークライアント
    /// </param>
    public AzureBlobProductImageStorage(
        BlobContainerClient containerClient)
    {
        _containerClient =
            containerClient;
    }

    /// <summary>
    /// 商品画像をAzure Blob Storageへ保存する
    /// </summary>
    /// <param name="imageStream">
    /// 保存する画像のストリーム
    /// </param>
    /// <param name="productName">
    /// Blobの保存名に使用する商品名
    /// </param>
    /// <param name="contentType">
    /// 画像のContent-Type
    /// </param>
    /// <param name="cancellationToken">
    /// キャンセルトークン
    /// </param>
    /// <returns>
    /// 保存した画像の公開URL
    /// </returns>
    public async Task<string> UploadAsync(
        Stream imageStream,
        string productName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(
            imageStream);

        if (!imageStream.CanRead)
        {
            throw new ArgumentException(
                "画像ストリームを読み取れません。",
                nameof(imageStream));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(
            productName);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            contentType);

        string extension =
            GetExtension(
                contentType);

        string safeProductName =
            CreateSafeProductName(
                productName);

        /*
         * 商品名だけでは重複する可能性があるため、
         * GUIDを組み合わせて一意なBlob名を作る。
         */
        string blobName =
            $"{safeProductName}_" +
            $"{Guid.NewGuid():N}" +
            $"{extension}";

        BlobClient blobClient =
            _containerClient.GetBlobClient(
                blobName);

        var uploadOptions =
            new BlobUploadOptions
            {
                HttpHeaders =
                    new BlobHttpHeaders
                    {
                        ContentType =
                            contentType,

                        CacheControl =
                            "public,max-age=31536000,immutable",
                    },
            };

        await blobClient.UploadAsync(
            imageStream,
            uploadOptions,
            cancellationToken);

        return blobClient.Uri.GetLeftPart(
            UriPartial.Path); //石原:変更 SASトークンを除いた画像URLだけを返す
    }

    /// <summary>
    /// Azure Blob Storageから商品画像を削除する
    /// </summary>
    /// <param name="imageUrl">
    /// 削除する画像のURL
    /// </param>
    /// <param name="cancellationToken">
    /// キャンセルトークン
    /// </param>
    public async Task DeleteAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        if (
            !Uri.TryCreate(
                imageUrl,
                UriKind.Absolute,
                out Uri? imageUri))
        {
            throw new ArgumentException(
                "画像URLの形式が正しくありません。",
                nameof(imageUrl));
        }

        Uri containerUri =
            _containerClient.Uri;

        if (
            !string.Equals(
                imageUri.Host,
                containerUri.Host,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "商品画像用ストレージのURLではありません。",
                nameof(imageUrl));
        }

        string containerPath =
            containerUri.AbsolutePath
                .TrimEnd('/');

        string expectedPrefix =
            $"{containerPath}/";

        if (
            !imageUri.AbsolutePath.StartsWith(
                expectedPrefix,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "商品画像用コンテナーのURLではありません。",
                nameof(imageUrl));
        }

        string blobName =
            Uri.UnescapeDataString(
                imageUri.AbsolutePath[
                    expectedPrefix.Length..]);

        if (string.IsNullOrWhiteSpace(
                blobName))
        {
            throw new ArgumentException(
                "画像URLからBlob名を取得できませんでした。",
                nameof(imageUrl));
        }

        await _containerClient
            .DeleteBlobIfExistsAsync(
                blobName,
                DeleteSnapshotsOption
                    .IncludeSnapshots,
                cancellationToken:
                    cancellationToken);
    }

    /// <summary>
    /// 商品名をBlob名に使用できる形式へ変換する
    /// </summary>
    /// <param name="productName">
    /// 商品名
    /// </param>
    /// <returns>
    /// Blob名として使用する商品名
    /// </returns>
    private static string CreateSafeProductName(
        string productName)
    {
        var builder =
            new StringBuilder();

        foreach (
            char character in
            productName.Trim())
        {
            if (
                char.IsLetterOrDigit(
                    character) ||
                character == '-' ||
                character == '_')
            {
                builder.Append(
                    character);

                continue;
            }

            if (char.IsWhiteSpace(
                    character))
            {
                builder.Append('-');
            }
        }

        string safeProductName =
            builder.ToString();

        if (
            string.IsNullOrWhiteSpace(
                safeProductName))
        {
            return "product";
        }

        return safeProductName;
    }

    /// <summary>
    /// Content-Typeに対応する拡張子を取得する
    /// </summary>
    /// <param name="contentType">
    /// 画像のContent-Type
    /// </param>
    /// <returns>
    /// 画像の拡張子
    /// </returns>
    private static string GetExtension(
        string contentType)
    {
        return contentType
            .ToLowerInvariant() switch
        {
            "image/jpeg" =>
                ".jpg",

            "image/png" =>
                ".png",

            _ => throw new ArgumentException(
                "PNG形式またはJPEG形式の画像を指定してください。",
                nameof(contentType)),
        };
    }
}