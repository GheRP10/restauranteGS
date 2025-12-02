using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_usu")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_usu")]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Column("email_usu")]
        public string Email { get; set; } = string.Empty;

        [JsonIgnore] 
        [Column("senhahash_usu")]
        public string SenhaHash { get; set; } = string.Empty;

        [Column("datacriacao_usu")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}