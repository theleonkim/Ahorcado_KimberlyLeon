using Ahorcado_KimberlyLeon.Models;

namespace Ahorcado_KimberlyLeon.Services
{
    public class JuegoService
    {
        public Partida CrearPartida(Jugador jugador, string nivelTexto)
        {
            var dificultad = ParseDificultad(nivelTexto);

            var partida = new Partida
            {
                Jugador = jugador,
                Dificultad = dificultad,
                Palabra = ObtenerPalabraPorDificultad(dificultad)
            };

            return partida;
        }

        private Dificultad ParseDificultad(string nivelTexto)
        {
            if (string.IsNullOrWhiteSpace(nivelTexto))
                return Dificultad.Normal;

            var s = nivelTexto.Trim().ToLowerInvariant();

            if (s == "facil" || s == "fácil")
                return Dificultad.Facil;

            if (s == "dificil" || s == "difícil")
                return Dificultad.Dificil;

            // default
            return Dificultad.Normal;
        }

        private Palabra ObtenerPalabraPorDificultad(Dificultad dificultad)
        {
            // TODO: reemplazar por tu lógica real de selección según dificultad
            return new Palabra { Texto = "ejemplo" };
        }

        public string ObtenerNivelTexto(Partida partida)
        {
            return partida == null ? null : partida.Dificultad.ToString();
        }
    }
}
