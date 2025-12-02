using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CriarReservaDto
    {
        [Required(ErrorMessage = "O ID da mesa é obrigatório")]
        public int MesaId { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "A data e hora de início são obrigatórias")]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "O número de pessoas deve ser entre 1 e 20")]
        public int NumeroPessoas { get; set; }
    }
}