using Ahorcado_KimberlyLeon.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Ahorcado_KimberlyLeon.Data
{
    public class AhorcadoContext : DbContext
    {
        public AhorcadoContext() : base("name=AhorcadoDb") { }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Palabra> Palabras { get; set; }
        public DbSet<Partida> Partidas { get; set; }   // ← FALTABA

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Jugador>().ToTable("Jugadores");
            modelBuilder.Entity<Palabra>().ToTable("Palabras");
            modelBuilder.Entity<Partida>().ToTable("Partidas");   // ← map explícito
            base.OnModelCreating(modelBuilder);
        }
    }
}
