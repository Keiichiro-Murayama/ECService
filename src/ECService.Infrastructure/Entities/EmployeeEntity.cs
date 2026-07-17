using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECService.Infrastructure.Entities
{
    [Table("employee")]
    public class EmployeeEntity
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
        [Column("employee_uuid")]
        public Guid EmployeeUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 部署ID（FK）
        /// </summary>
        [Column("department_id")]
        public int DepartmentId { get; set; }

        /// <summary>
        /// 社員名
        /// </summary>
        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 社員名（かな）
        /// </summary>
        [Required]
        [Column("name_kana")]//石原:kanaからname_kanaに変更
        [MaxLength(100)]
        public string Kana { get; set; } = string.Empty;

        //ナビゲーションプロパティ
        public DepartmentEntity Department { get; set; } = null!;
    }
}