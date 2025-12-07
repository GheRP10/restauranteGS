using backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==================================================================
// 1. CONFIGURAÇÃO DE SERVIÇOS (Injeção de Dependência)
// ==================================================================

// A. Configurar CORS (Permite que o Front-end acesse o Back-end)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Aceita requisições de qualquer URL (localhost:5173, etc)
              .AllowAnyMethod()  // Aceita GET, POST, PUT, DELETE
              .AllowAnyHeader(); // Aceita todos os cabeçalhos
    });
});

// B. Adicionar Controllers
builder.Services.AddControllers();

// C. Configurar Banco de Dados (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// D. Configurar Swagger (Documentação)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// E. Configurar RabbitMQ Service
builder.Services.AddScoped<backend.Services.RabbitMQService>();

var app = builder.Build();

// ==================================================================
// 2. CONFIGURAÇÃO DO PIPELINE (Middleware)
// ==================================================================

// Se estiver em desenvolvimento, ativa o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// A. Aplicar CORS 
app.UseCors("AllowAll");

// B. Autorização (Se houver no futuro)
app.UseAuthorization();

// C. Mapear os Endpoints
app.MapControllers();

// D. Rodar a Aplicação
app.Run();