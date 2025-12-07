using backend.Data;
using backend.Models; // Adicionado para reconhecer Restaurante e Mesa
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

// Configura o PostgreSQL com a Connection String do appsettings ou Variável de Ambiente
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Serviço do RabbitMQ
builder.Services.AddScoped<backend.Services.RabbitMQService>();

var app = builder.Build();

// ==================================================================
// 2. AUTO-MIGRATION & SEED (O CORRETOR DO BANCO) 🛠️
// ==================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // 1. CRIA AS TABELAS AUTOMATICAMENTE NA NUVEM
        context.Database.EnsureCreated();

        // 2. CRIA DADOS INICIAIS (SE ESTIVER VAZIO)
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
            context.SaveChanges(); // Salva para gerar o ID

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();