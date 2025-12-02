using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("reservas")]
    public class Reserva
    {
        [Key]
        [Column("id_rev")]
        public long Id { get; set; } 
       
        [Column("id_mes_rev")]
        public int MesaId { get; set; }
        
        [ForeignKey("MesaId")]
        public Mesa? Mesa { get; set; }

        [Column("id_usu_rev")]
        public int UsuarioId { get; set; }
        
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Column("datahorainicio_rev")]
        public DateTime DataHoraInicio { get; set; }

        [Column("datahorafim_rev")]
        public DateTime DataHoraFim { get; set; }

        [Column("numeropessoas_rev")]
        public int NumeroPessoas { get; set; }

        [MaxLength(20)]
        [Column("status_rev")]
        public string Status { get; set; } = "CONFIRMADA";

        [Column("datacriacao_rev")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}