using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ECService.Infrastructure.Entities
{
    /// <summary>
    /// 部署エンティティ
    /// </summary>
    [Table("department")]
    public class DepartmentEntity
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
        [Column("department_uuid")]
        public Guid DepartmentUuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 部署名
        /// </summary>
        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

    }
}