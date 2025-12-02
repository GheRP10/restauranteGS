using backend.Models; // Referência aos seus Models
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    // Equivalente ao "BancoContext" do seu print
    public class AppDbContext : DbContext
    {
        // Construtor padrão (igual ao do seu print)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Mapeamento das Tabelas (seus DbSets)
        public DbSet<Restaurante> Restaurantes { get; set; }
        public DbSet<Mesa> Mesas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }

        // Configurações avançadas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AQUI ESTÁ A DIFERENÇA SÊNIOR:
            // Em vez de só popular dados, estamos tunando o banco.
            
            // Criando o Índice Composto na tabela Reservas
            // Isso diz ao banco: "Crie um atalho para buscar por Mesa + Horário + Status"
            modelBuilder.Entity<Reserva>()
                .HasIndex(r => new { r.MesaId, r.DataHoraInicio, r.DataHoraFim, r.Status })
                .HasDatabaseName("IDX_Reservas_Disponibilidade");
        }
    }
}