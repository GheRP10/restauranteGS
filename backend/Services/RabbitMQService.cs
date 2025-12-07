using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace backend.Services
{
    public class RabbitMQService
    {
        // Configurações hardcoded para facilitar (em prod, iria no appsettings)
        private const string QUEUE_NAME = "reservas_notificacoes";
        private readonly ConnectionFactory _factory;

        public RabbitMQService()
        {
            _factory = new ConnectionFactory 
            { 
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
        }

        public async Task EnviarNotificacaoAsync(object dados)
        {
            // 1. Conectar ao RabbitMQ
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            // 2. Garantir que a fila existe (Idempotência)
            // Se a fila não existir, ele cria. Se já existir, ele só usa.
            await channel.QueueDeclareAsync(
                queue: QUEUE_NAME,
                durable: true,      // Se o RabbitMQ reiniciar, a fila continua lá
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // 3. Preparar a mensagem (JSON -> Bytes)
            var json = JsonSerializer.Serialize(dados);
            var body = Encoding.UTF8.GetBytes(json);

            // 4. Publicar a mensagem
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QUEUE_NAME,
                body: body);
        }
    }
}