using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication4.Models
{
    public sealed class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public string Status { get; set; }
        public DateTime? LastLogin { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
    }
}