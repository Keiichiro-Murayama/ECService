using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 商品を表すドメインオブジェクト
/// 商品カテゴリを参照し、商品在庫を内包する集約ルート
/// </summary>
public class Product : Entity
{
    /// <summary>
    /// 商品UUID
    /// </summary>
    public string ProductUuid { get; private set; }

    /// <summary>
    /// 商品名
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// 価格
    /// </summary>
    public int Price { get; private set; }

    /// <summary>
    /// 画像URL
    /// </summary>
    public string ImageUrl { get; private set; }

    /// <summary>
    /// 削除フラグ
    /// 0: 未削除、1: 削除済み
    /// </summary>
    public int DeleteFlg { get; private set; }

    /// <summary>
    /// 商品カテゴリ
    /// </summary>
    public ProductCategory ProductCategory { get; private set; }

    /// <summary>
    /// 商品在庫
    /// </summary>
    public ProductStock ProductStock { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => ProductUuid;

    /// <summary>
    /// 商品名の最大文字数
    /// </summary>
    private const int NameMaxLength = 100;

    /// <summary>
    /// 画像URLの最大文字数
    /// </summary>
    private const int ImageUrlMaxLength = 200;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Product(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        int deleteFlg,
        ProductCategory productCategory,
        ProductStock productStock)
    {
        ProductUuid = productUuid;
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
        DeleteFlg = deleteFlg;
        ProductCategory = productCategory;
        ProductStock = productStock;
    }

    /// <summary>
    /// 新しい商品を生成する
    /// 新規作成時は削除フラグを0にする
    /// </summary>
    public static Product Create(
        string name,
        int price,
        string imageUrl,
        ProductCategory productCategory,
        int quantity)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateImageUrl(imageUrl);
        ValidateProductCategory(productCategory);

        var productUuid = Guid.NewGuid().ToString();
        var productStock = ProductStock.Create(quantity);

        return new Product(
            productUuid,
            name,
            price,
            imageUrl,
            0,
            productCategory,
            productStock);
    }

    /// <summary>
    /// 既存の商品を復元する
    /// </summary>
    public static Product Restore(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        int deleteFlg,
        ProductCategory productCategory,
        ProductStock productStock)
    {
        ValidateUuid(productUuid, nameof(productUuid));
        ValidateName(name);
        ValidatePrice(price);
        ValidateImageUrl(imageUrl);
        ValidateDeleteFlg(deleteFlg);
        ValidateProductCategory(productCategory);
        ValidateProductStock(productStock);

        return new Product(
            productUuid,
            name,
            price,
            imageUrl,
            deleteFlg,
            productCategory,
            productStock);
    }

    /// <summary>
    /// 商品名を変更する
    /// </summary>
    public void ChangeName(string name)
    {
        ValidateName(name);
        Name = name;
    }

    /// <summary>
    /// 価格を変更する
    /// </summary>
    public void ChangePrice(int price)
    {
        ValidatePrice(price);
        Price = price;
    }

    /// <summary>
    /// 画像URLを変更する
    /// </summary>
    public void ChangeImageUrl(string imageUrl)
    {
        ValidateImageUrl(imageUrl);
        ImageUrl = imageUrl;
    }

    /// <summary>
    /// 商品カテゴリを変更する
    /// </summary>
    public void ChangeCategory(ProductCategory productCategory)
    {
        ValidateProductCategory(productCategory);
        ProductCategory = productCategory;
    }

    /// <summary>
    /// 商品在庫を変更する
    /// </summary>
    public void ChangeStock(ProductStock productStock)
    {
        ValidateProductStock(productStock);
        ProductStock = productStock;
    }

    /// <summary>
    /// 商品を削除済みにする
    /// </summary>
    public void Delete()
    {
        DeleteFlg = 1;
    }

    /// <summary>
    /// 削除済みか判定する
    /// </summary>
    public bool IsDeleted()
    {
        return DeleteFlg == 1;
    }

    /// <summary>
    /// 商品名を検証する
    /// </summary>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("商品名は必須です。", nameof(name));
        }

        if (name.Length > NameMaxLength)
        {
            throw new DomainException(
                $"商品名は{NameMaxLength}文字以内で指定してください。", nameof(name));
        }
    }

    /// <summary>
    /// 価格を検証する
    /// </summary>
    private static void ValidatePrice(int price)
    {
        if (price < 0)
        {
            throw new DomainException("価格は0以上で指定してください。", nameof(price));
        }
    }

    /// <summary>
    /// 画像URLを検証する
    /// </summary>
    private static void ValidateImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new DomainException("画像URLは必須です。", nameof(imageUrl));
        }

        if (imageUrl.Length > ImageUrlMaxLength)
        {
            throw new DomainException(
                $"画像URLは{ImageUrlMaxLength}文字以内で指定してください。", nameof(imageUrl));
        }
    }

    /// <summary>
    /// 削除フラグを検証する
    /// </summary>
    private static void ValidateDeleteFlg(int deleteFlg)
    {
        if (deleteFlg != 0 && deleteFlg != 1)
        {
            throw new DomainException("削除フラグは0または1で指定してください。", nameof(deleteFlg));
        }
    }

    /// <summary>
    /// 商品カテゴリを検証する
    /// </summary>
    private static void ValidateProductCategory(ProductCategory productCategory)
    {
        if (productCategory is null)
        {
            throw new DomainException("商品カテゴリは必須です。", nameof(productCategory));
        }
    }

    /// <summary>
    /// 商品在庫を検証する
    /// </summary>
    private static void ValidateProductStock(ProductStock productStock)
    {
        if (productStock is null)
        {
            throw new DomainException("商品在庫は必須です。", nameof(productStock));
        }
    }

    /// <summary>
    /// UUIDを検証する
    /// </summary>
    private static void ValidateUuid(string uuid, string paramName)
    {
        if (string.IsNullOrWhiteSpace(uuid))
        {
            throw new DomainException("識別Idは必須です。", paramName);
        }

        if (!Guid.TryParse(uuid, out _))
        {
            throw new DomainException("識別Idの形式が不正です。", paramName);
        }
    }
}