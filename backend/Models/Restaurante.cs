using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("restaurantes")]
    public class Restaurante
    {
        [Key]
        [Column("id_res")] 
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_res")]
        public string Nome { get; set; } = string.Empty;

        public string Endereco { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;

        [Column("tempopadrao_res")]
        public int TempoPadraoReserva { get; set; } 

        [Column("horaabertura_res")]
        public TimeSpan HoraAbertura { get; set; }

        [Column("horafechamento_res")]
        public TimeSpan HoraFechamento { get; set; }
    }
}