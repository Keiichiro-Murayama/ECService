using ECService.Domain.Exceptions;

namespace ECService.Domain.Models;

/// <summary>
/// 商品を表すドメインオブジェクト
/// ProductCategoryを参照し、ProductStockを保持する
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
    /// 商品カテゴリ
    /// </summary>
    public ProductCategory ProductCategory { get; private set; }

    /// <summary>
    /// 削除フラグ
    /// 0: 未削除、1: 削除済み
    /// </summary>
    public int DeleteFlg { get; private set; }

    /// <summary>
    /// 商品在庫
    /// </summary>
    public ProductStock ProductStock { get; private set; }

    /// <summary>
    /// 同一性判定に使用する識別子
    /// </summary>
    protected override string Identity => ProductUuid;

    /// <summary>
    /// 商品名の最小文字数
    /// </summary>
    private const int NameMinLength = 2;

    /// <summary>
    /// 商品名の最大文字数
    /// </summary>
    private const int NameMaxLength = 20;

    /// <summary>
    /// 価格の最小値
    /// </summary>
    private const int PriceMinValue = 0;

    /// <summary>
    /// 価格の最大値
    /// </summary>
    private const int PriceMaxValue = 1000000;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    private Product(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        ProductCategory productCategory,
        int deleteFlg,
        ProductStock productStock)
    {
        ProductUuid = productUuid;
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
        ProductCategory = productCategory;
        DeleteFlg = deleteFlg;
        ProductStock = productStock;
    }

    /// <summary>
    /// 新しい商品を生成する
    /// </summary>
    public static Product Create(
        string name,
        int price,
        string imageUrl,
        ProductCategory productCategory,
        ProductStock productStock)
    {
        ValidateName(name);
        ValidatePrice(price);
        ValidateImageUrl(imageUrl);
        ValidateCategory(productCategory);
        ValidateStock(productStock);

        var productUuid = Guid.NewGuid().ToString();

        return new Product(
            productUuid,
            name,
            price,
            imageUrl,
            productCategory,
            0,
            productStock);
    }
    public static Product Restore(
        string productUuid,
        string name,
        int price,
        string imageUrl,
        ProductCategory productCategory,
         int deleteFlg,
        ProductStock productStock)
    {

        ValidateName(name);
        ValidatePrice(price);
        ValidateImageUrl(imageUrl);
        ValidateCategory(productCategory);
        ValidateStock(productStock);

        return new Product(
        productUuid,
        name,
        price,
        imageUrl,
        productCategory,
        deleteFlg,
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
        ValidateCategory(productCategory);

        ProductCategory = productCategory;
    }

    /// <summary>
    /// 商品在庫を変更する
    /// </summary>
    public void ChangeStock(ProductStock productStock)
    {
        ValidateStock(productStock);

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
    public bool IsDelete()
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
            throw new DomainException("商品名を入力してください", nameof(name));
        }

        if (name.Length < NameMinLength || name.Length > NameMaxLength)
        {
            throw new DomainException("商品名は2～20文字で入力してください", nameof(name));
        }
    }

    /// <summary>
    /// 価格を検証する
    /// </summary>
    private static void ValidatePrice(int price)
    {
        if (price < PriceMinValue || price > PriceMaxValue)
        {
            throw new DomainException("価格は100万円以下で入力してください", nameof(price));
        }
    }

    /// <summary>
    /// 画像URLを検証する
    /// </summary>
    private static void ValidateImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            throw new DomainException("画像をアップロードしてください", nameof(imageUrl));
        }
    }

    /// <summary>
    /// 商品カテゴリを検証する
    /// </summary>
    private static void ValidateCategory(ProductCategory productCategory)
    {
        if (productCategory is null)
        {
            throw new DomainException("カテゴリを選択してください", nameof(productCategory));
        }
    }

    /// <summary>
    /// 商品在庫を検証する
    /// </summary>
    private static void ValidateStock(ProductStock productStock)
    {
        if (productStock is null)
        {
            throw new DomainException("在庫数を入力してください", nameof(productStock));
        }
    }
}