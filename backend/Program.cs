using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// 1. CONFIGURAÇÃO DE SERVIÇOS
// ==================================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// ------------------------------------------------------------------
// CONEXÃO COM BANCO (LOCAL + RAILWAY)
// ------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Se existir DATABASE_URL (Railway), ela sobrescreve a conexão local
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Formato vindo do Railway: postgres://user:pass@host:port/dbname
    var databaseUri = new Uri(databaseUrl);
    var userInfo = databaseUri.UserInfo.Split(':');

    connectionString =
        $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Swagger e RabbitMQ
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<backend.Services.RabbitMQService>();

var app = builder.Build();

// ==================================================================
// 2. CRIAÇÃO AUTOMÁTICA DO BANCO E DADOS (SEED COMPLETO)
// ==================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Em desenvolvimento pode recriar o banco do zero
        if (app.Environment.IsDevelopment())
        {
            context.Database.EnsureDeleted();
        }

        // Em produção (Railway) só garante que o banco existe
        context.Database.EnsureCreated();

        // Popula os dados se não existir nenhum restaurante
        if (!context.Restaurantes.Any())
        {
            Console.WriteLine("--> Populando banco de dados...");

            // 1. Cria o Restaurante
            var restaurante = new Restaurante
            {
                Nome = "Bistrô do Dev",
                Endereco = "Nuvem Railway, 123",
                Telefone = "11 99999-9999",
                TempoPadraoReserva = 90,
                HoraAbertura = new TimeSpan(18, 0, 0),    // Abre às 18h
                HoraFechamento = new TimeSpan(23, 59, 0)  // Fecha às 23h59
            };
            context.Restaurantes.Add(restaurante);
            context.SaveChanges();

            var mesas = new List<Mesa>
            {
                new Mesa { NumeroMesa = "M1", Capacidade = 2, RestauranteId = restaurante.Id },
                new Mesa { NumeroMesa = "M2", Capacidade = 4, RestauranteId = restaurante.Id },
                new Mesa { NumeroMesa = "M3", Capacidade = 6, RestauranteId = restaurante.Id },
                new Mesa { NumeroMesa = "M4", Capacidade = 8, RestauranteId = restaurante.Id },
                new Mesa { NumeroMesa = "M5", Capacidade = 10, RestauranteId = restaurante.Id }
            };
            context.Mesas.AddRange(mesas);
            context.SaveChanges(); 

            Console.WriteLine("--> Dados criados com sucesso: Restaurante + 5 Mesas.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro fatal ao criar/atualizar o banco de dados.");
    }
}

// ==================================================================
// 3. PIPELINE
// ==================================================================

// Swagger mesmo em produção para facilitar testes
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
