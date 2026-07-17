using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECService.Infrastructure.Entities
{
    [Table("orders_detail")]
    public class OrdersDetailEntity
    {
        /// <summary>
        /// DB上のID（自動採番）
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 注文ID（FK）
        /// </summary>
        [Column("order_id")]
        public int OrderId { get; set; }

        /// <summary>
        /// 商品ID（FK）
        /// </summary>
        [Column("product_id")]
        public int ProductId { get; set; }

        /// <summary>
        /// 注文数
        /// </summary>
        [Column("count")]
        public int Count { get; set; }

        // ナビゲーションプロパティ
        public OrdersEntity Order { get; set; } = null!;
        public ProductEntity Product { get; set; } = null!;
    }
}
