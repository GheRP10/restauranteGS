using backend.Data;
using backend.DTOs;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RabbitMQService _rabbitMQ; // Serviço de Mensageria

        public ReservasController(AppDbContext context, RabbitMQService rabbitMQ)
        {
            _context = context;
            _rabbitMQ = rabbitMQ;
        }

        [HttpPost]
        public async Task<IActionResult> CriarReserva([FromBody] CriarReservaDto dto)
        {
            // 1. Garantir que a DataHoraInicio está em UTC
            DateTime dataInicioUtc = DateTime.SpecifyKind(dto.DataHoraInicio, DateTimeKind.Utc);

            // ==================================================================================
            // REGRAS DE NEGÓCIO
            // ==================================================================================
            
            if (dataInicioUtc < DateTime.UtcNow)
            {
                return BadRequest("RN.CRS.001 - Não é possível fazer reserva para datas passadas.");
            }

            // ==================================================================================

            // 2. Obter Mesa e Restaurante Associado
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

            // 3. Calcular DataHoraFim com base no Tempo Padrão do Restaurante
            int tempoPadrao = mesa.Restaurante.TempoPadraoReserva;
            DateTime dataFimUtc = dataInicioUtc.AddMinutes(tempoPadrao);

            // 4. Iniciar Transação (Bloqueio)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 5. Verificar Conflito de Horário (Overbooking)
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

                // 6. Criar e Salvar Nova Reserva   
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

                // 7. Confirmar Transação
                await transaction.CommitAsync();

                // ========================================================
                // 8. PUBLICAR NOTIFICAÇÃO NO RABBITMQ (Assíncrono) 
                // ========================================================
                var dadosNotificacao = new 
                {
                    ReservaId = novaReserva.Id,
                    Data = novaReserva.DataHoraInicio,
                    Pessoas = novaReserva.NumeroPessoas,
                    Mensagem = "Reserva confirmada com sucesso!"
                };

                await _rabbitMQ.EnviarNotificacaoAsync(dadosNotificacao);
                // ========================================================

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