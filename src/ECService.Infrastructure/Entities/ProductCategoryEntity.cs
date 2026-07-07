using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECService.Infrastructure.Entities
{
    /// <summary>
    /// 商品カテゴリ
    /// </summary>
    [Table("product_category")]
    public class ProductCategoryEntity
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
        [Column("category_uuid")]
        public Guid CategoryUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// カテゴリ名
        /// </summary>
        [Required]
        [MaxLength(20)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

    }
}