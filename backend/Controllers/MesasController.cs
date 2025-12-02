using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MesasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MesasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Mesas/disponibilidade?dataHora=2025-12-01T19:00:00&numeroPessoas=2
        [HttpGet("disponibilidade")]
        public async Task<IActionResult> ObterMesasDisponiveis([FromQuery] DateTime dataHora, [FromQuery] int numeroPessoas)
        {
            // Converter entrada para UTC
            DateTime dataInicioUtc = DateTime.SpecifyKind(dataHora, DateTimeKind.Utc);

            // Pegar configuração de tempo do restaurante (Pegando o primeiro para simplificar)
            var restaurante = await _context.Restaurantes.FirstOrDefaultAsync();
            
            if (restaurante == null)
            {
                return StatusCode(500, "Nenhum restaurante cadastrado no banco.");
            }        

            int tempoPadrao = restaurante.TempoPadraoReserva;
            DateTime dataFimUtc = dataInicioUtc.AddMinutes(tempoPadrao);

            // Query de Disponibilidade
            // Seleciona mesas que CABEM as pessoas E que NÃO ESTÃO ocupadas naquele horário
            var mesasLivres = await _context.Mesas
                .Where(m => m.Capacidade >= numeroPessoas) 
                .Where(m => !_context.Reservas.Any(r => 
                    r.MesaId == m.Id &&
                    r.Status != "CANCELADA" &&
                    r.DataHoraInicio < dataFimUtc && 
                    r.DataHoraFim > dataInicioUtc
                ))
                .ToListAsync();

            return Ok(mesasLivres);
        }
    }
}