using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECService.Infrastructure.Entities
{
    public class EmployeeAccountEntity
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
        [Column("account_uuid")]
        public Guid AccountUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 社員ID（FK）
        /// </summary>
        [Column("employee_id")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// 社員アカウント名
        /// </summary>
        [Required]
        [Column("name")]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 社員アカウントパスワード
        /// </summary>
        [Required]
        [Column("password")]
        [MaxLength(200)]
        public string Password { get; set; } = string.Empty;

        //ナビゲーションプロパティ
        public EmployeeEntity Employee { get; set; } = null!;
    }
}