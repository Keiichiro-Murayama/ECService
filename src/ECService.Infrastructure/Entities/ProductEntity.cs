using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECService.Infrastructure.Entities
{
    /// <summary>
    /// 商品エンティティ
    /// </summary>
    [Table("product")]
    public class ProductEntity
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
        [Column("product_uuid")]
        public Guid ProductUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// カテゴリID（FK）
        /// </summary>
        [Column("product_category_id")]
        public int ProductCategoryId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 価格
        /// </summary>
        [Column("price")]
        public int Price { get; set; }

        /// <summary>
        /// 商品画像
        /// </summary>
        [MaxLength(200)]
        [Column("image_url")]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("delete_flag")]
        public int DeleteFlag { get; set; } = 0;

        //ナビゲーションプロパティ
        public ProductCategoryEntity ProductCategory { get; set; } = null!;
        public ProductStockEntity ProductStock { get; set; } = null!;
    }
}