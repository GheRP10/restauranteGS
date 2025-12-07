using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    // 1. Mantém o nome da tabela no banco como "restaurantes"
    [Table("restaurantes")]
    public class Restaurante
    {
        [Key]
        [Column("id_res")] // 2. Mapeia "Id" do C# para "id_res" do Banco
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nome_res")] // Mapeia "Nome" para "nome_res"
        public string Nome { get; set; } = string.Empty;
        
        // --- NOVOS CAMPOS (Criam colunas novas se não existirem) ---
        [Column("endereco_res")]
        public string Endereco { get; set; } = string.Empty;

        [Column("telefone_res")]
        public string Telefone { get; set; } = string.Empty;
        // -----------------------------------------------------------

        [Column("tempopadrao_res")]
        public int TempoPadraoReserva { get; set; } = 90;

        // Estes campos podem ser opcionais ou removidos se não usar agora
        [Column("horaabertura_res")]
        public TimeSpan HoraAbertura { get; set; }

        [Column("horafechamento_res")]
        public TimeSpan HoraFechamento { get; set; }

        [JsonIgnore]
        public List<Mesa>? Mesas { get; set; }
    }
}