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
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
}

if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
{
    var databaseUri = new Uri(connectionString);
    var userInfo = databaseUri.UserInfo.Split(':');
    
    connectionString = $"Host={databaseUri.Host};Port={databaseUri.Port};Database={databaseUri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true";
}
// ------------------------------------------------------------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<backend.Services.RabbitMQService>();

var app = builder.Build();

// ==================================================================
// 2. AUTO-MIGRATION & SEED (CRIA TABELAS E DADOS)
// ==================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Garante que o banco foi criado
        context.Database.EnsureCreated();

        // Se não houver restaurantes, cria os dados padrão
        if (!context.Restaurantes.Any())
        {
            Console.WriteLine("--> Criando dados iniciais no Banco (Seed)...");

            var restaurante = new Restaurante
            {
                Nome = "Restaurante GS",
                Endereco = "Nuvem Railway",
                Telefone = "9999-9999",
                TempoPadraoReserva = 90
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
            
            Console.WriteLine("--> Banco populado com sucesso!");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao criar/popular o banco de dados.");
    }
}

// ==================================================================
// 3. PIPELINE
// ==================================================================

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();