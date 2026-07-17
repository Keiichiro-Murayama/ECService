using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECService.Infrastructure.Entities
{
    [Table("customer")]
    public class CustomerEntity
    {
        /// <summary>
        /// DB上のID（自動採番）
        /// </summary>
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 顧客識別ID
        /// </summary>
        [Required]
        [Column("customer_uuid")]
        public Guid CustomerUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 顧客名
        /// </summary>
        [Required]
        [Column("name")]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 住所1
        /// </summary>
        [Required]
        [Column("address1")]
        [MaxLength(100)]
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        /// 住所2
        /// </summary>
        [Required]
        [Column("address2")]
        [MaxLength(100)]
        public string Address2 { get; set; } = string.Empty;

        /// <summary>
        /// 電話番号
        /// </summary>
        [Required]
        [Column("phone_number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Required]
        [Column("mail_address")]
        [MaxLength(200)]
        public string MailAddress { get; set; } = string.Empty;

        /// <summary>
        /// アカウント名
        /// </summary>
        [Required]
        [Column("username")]
        [MaxLength(30)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// パスワード
        /// </summary>
        [Required]
        [Column("password")]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 登録日
        /// </summary>
        [Required]
        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        //ナビゲーションプロパティ
        public ICollection<OrdersEntity>? OrdersEntities { get; set; } = new List<OrdersEntity>();
    }
}