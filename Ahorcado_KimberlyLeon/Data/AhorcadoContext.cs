using System.Data.Entity;
using Ahorcado_KimberlyLeon.Models;

namespace Ahorcado_KimberlyLeon.Data
{
    public class AhorcadoContext : DbContext
    {
        // Usa la conexión del Web.config: name="AhorcadoContext"
        public AhorcadoContext() : base("AhorcadoContext") { }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Palabra> Palabras { get; set; }
        public DbSet<Partida> Partidas { get; set; }
    }
}
