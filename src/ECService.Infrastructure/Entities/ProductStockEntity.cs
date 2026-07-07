using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECService.Infrastructure.Entities;

/// <summary>
/// 商品在庫
/// </summary>
[Table("product_stock")]
public class ProductStockEntity
{
    /// <summary>
    /// DB上のID（自動採番）
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// UUID　自動採番
    /// </summary>
    [Required]
    [Column("stock_uuid")]
    public Guid StockUuid { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品ID(FK) 
    /// </summary>
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 商品在庫
    /// </summary>
    [Required]
    [Column("quantity")]
    public int Quantity { get; set; }

    //ナビゲーションプロパティ
    public ProductEntity Product { get; set; } = null!;


}
