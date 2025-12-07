using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotificationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ConnectionFactory _factory;
    private const string QUEUE_NAME = "reservas_notificacoes";

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
        _factory = new ConnectionFactory { HostName = "localhost" };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = await _factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: QUEUE_NAME, durable: true, exclusive: false, autoDelete: false);

        _logger.LogInformation(" [*] Aguardando mensagens na fila {Queue}...", QUEUE_NAME);

        var consumer = new AsyncEventingBasicConsumer(channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            // SIMULAÃO DE ENVIO DE E-MAIL 📧
            _logger.LogInformation($"[📧 EMAIL ENVIADO] Processando mensagem: {message}");
            
            // Simula um delay de processamento (ex: servidor de e-mail lento)
            await Task.Delay(1000); 
        };

        await channel.BasicConsumeAsync(queue: QUEUE_NAME, autoAck: true, consumer: consumer);

        // Mantém o serviço rodando até ser cancelado
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}