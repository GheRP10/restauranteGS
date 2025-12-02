using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace backend.Models
{
    [Table("mesas")]
    public class Mesa
    {
        [Key]
        [Column("id_mes")]
        public int Id { get; set; }

        [Column("id_res_mes")]
        public int RestauranteId { get; set; }
        
        [JsonIgnore] 
        [ForeignKey("RestauranteId")]
        public Restaurante? Restaurante { get; set; }

        [Required]
        [MaxLength(10)]
        [Column("numeromesa_mes")]
        public string NumeroMesa { get; set; } = string.Empty;

        [Column("capacidade_mes")]
        public int Capacidade { get; set; }
    }
}