using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECService.Infrastructure.Entities
{
    [Table("orders")]
    public class OrdersEntity
    {
        /// <summary>
        /// DB上のID（自動採番）
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 注文識別ID
        /// </summary>
        [Required]
        [Column("order_uuid")]
        public Guid OrderUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 注文日
        /// </summary>
        [Required]
        [Column("order_date")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 合計金額
        /// </summary>
        [Column("amount_total")]
        public int AmountTotal { get; set; }

        /// <summary>
        /// 顧客ID（FK）
        /// </summary>
        [Column("customer_id")]
        public int CustomerId { get; set; }

        /// <summary>
        /// 注文ステータスID（FK）
        /// </summary>
        [Column("order_status_id")]
        public int OrderStatusId { get; set; }

        /// <summary>
        /// 支払い方法ID（FK）
        /// </summary>
        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        // ナビゲーションプロパティ
        public CustomerEntity Customer { get; set; } = null!;
        public OrderStatusEntity OrderStatus { get; set; } = null!;
        public PaymentMethodEntity PaymentMethod { get; set; } = null!;
        public ICollection<OrdersDetailEntity> OrdersDetails { get; set; } = new List<OrdersDetailEntity>();
    }
}
