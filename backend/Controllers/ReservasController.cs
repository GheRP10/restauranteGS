using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarReserva([FromBody] CriarReservaDto dto)
        {
            // Garantir que a DataHoraInicio está em UTC
            DateTime dataInicioUtc = DateTime.SpecifyKind(dto.DataHoraInicio, DateTimeKind.Utc);

            // ==================================================================================
            // REGRAS DE NEGÓCIO 
            // ==================================================================================
            
            if (dataInicioUtc < DateTime.UtcNow)
            {
                return BadRequest("RN.CRS.001 - Não é possível fazer reserva para datas passadas.");
            }

            // ==================================================================================

            // Obter Mesa e Restaurante Associado
            var mesa = await _context.Mesas
                .Include(m => m.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == dto.MesaId);

            if (mesa == null)
            {
                return NotFound("Mesa não encontrada.");
            }

            if (mesa.Restaurante == null)
            {
                return StatusCode(500, "Erro crítico: A mesa não pertence a um restaurante.");
            }

            // Calcular DataHoraFim com base no Tempo Padrão do Restaurante
            int tempoPadrao = mesa.Restaurante.TempoPadraoReserva;
            DateTime dataFimUtc = dataInicioUtc.AddMinutes(tempoPadrao);

            //  Iniciar Transação
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Verificar Conflito de Horário
                bool existeConflito = await _context.Reservas.AnyAsync(r =>
                    r.MesaId == dto.MesaId &&
                    r.Status != "CANCELADA" &&
                    r.DataHoraInicio < dataFimUtc && 
                    r.DataHoraFim > dataInicioUtc   
                );

                if (existeConflito)
                {
                    await transaction.RollbackAsync();
                    return Conflict("Esta mesa já está reservada para o horário selecionado.");
                }

                // Criar e Salvar Nova Reserva   
                var novaReserva = new Reserva
                {
                    MesaId = dto.MesaId,
                    UsuarioId = dto.UsuarioId,
                    DataHoraInicio = dataInicioUtc,
                    DataHoraFim = dataFimUtc,
                    NumeroPessoas = dto.NumeroPessoas,
                    Status = "CONFIRMADA",
                    DataCriacao = DateTime.UtcNow
                };

                _context.Reservas.Add(novaReserva);
                await _context.SaveChangesAsync();

                // Confirmar Transação
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(ListarReservas), new { id = novaReserva.Id }, novaReserva);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var msg = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, $"Erro interno: {msg}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarReservas()
        {
            var reservas = await _context.Reservas.ToListAsync();
            return Ok(reservas);
        }
    }
}