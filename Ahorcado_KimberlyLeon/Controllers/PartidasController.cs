using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ahorcado_KimberlyLeon.Controllers
{
    public class PartidasController : Controller
    {
        private readonly AhorcadoContext db = new AhorcadoContext();

        // GET: Partidas/Crear
        public async Task<ActionResult> Crear()
        {
            ViewBag.JugadorId = new SelectList(
                await db.Jugadores.AsNoTracking().ToListAsync(), "Id", "Nombre");
            ViewBag.Niveles = new SelectList(new[] { "Facil", "Normal", "Dificil" });
            return View();
        }

        // POST: Partidas/Jugar  → crea la partida y redirige al tablero
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Jugar(int JugadorId, string Nivel)
        {
            var jugador = await db.Jugadores.FindAsync(JugadorId);
            if (jugador == null) return HttpNotFound();

            // palabra aleatoria no usada
            var palabra = await db.Palabras
                                  .Where(p => !p.Usada)
                                  .OrderBy(p => Guid.NewGuid())
                                  .FirstOrDefaultAsync();
            if (palabra == null)
            {
                ViewBag.MensajeError = "No hay más palabras disponibles en el diccionario.";
                ViewBag.JugadorId = new SelectList(await db.Jugadores.AsNoTracking().ToListAsync(), "Id", "Nombre");
                ViewBag.Niveles = new SelectList(new[] { "Facil", "Normal", "Dificil" });
                return View("Crear");
            }

            var dificultad = ParseDificultad(Nivel);

            var partida = new Partida
            {
                JugadorId = jugador.Id,
                PalabraId = palabra.Id,
                Dificultad = dificultad,
                FechaInicio = DateTime.Now
            };

            db.Partidas.Add(partida);
            palabra.Usada = true; // si tu consigna lo pide
            await db.SaveChangesAsync();

            // Estado de juego en sesión
            Session["PartidaId"] = partida.Id;
            Session["PalabraSecreta"] = palabra.Texto;
            Session["PalabraOculta"] = InicializarOculta(palabra.Texto); // oculta solo letras
            Session["IntentosRestantes"] = 5;
            Session["LetrasAdivinadas"] = new List<char>();
            Session["JuegoTerminado"] = false;
            Session["Mensaje"] = null;

            return RedirectToAction("Jugar");   // ← tablero
        }

        // GET: Partidas/Jugar  → tablero (NECESARIA)
        [HttpGet]
        public ActionResult Jugar()
        {
            if (Session["PalabraSecreta"] == null)
                return RedirectToAction("Crear");

            ViewBag.PalabraOculta = Session["PalabraOculta"] as string;
            ViewBag.IntentosRestantes = Session["IntentosRestantes"];
            ViewBag.Mensaje = Session["Mensaje"];
            ViewBag.JuegoTerminado = Session["JuegoTerminado"];
            ViewBag.LetrasProbadas = string.Join(", ",
                (Session["LetrasAdivinadas"] as List<char>) ?? new List<char>());

            return View("Jugar"); // Views/Partidas/Jugar.cshtml
        }

        // GET: Partidas/AdivinarLetra?letra=X  → usamos GET para poder usar <a href=...>
        [HttpGet]
        public async Task<ActionResult> AdivinarLetra(char letra)
        {
            if (Session["JuegoTerminado"] is bool fin && fin)
                return RedirectToAction("Jugar");

            string palabraSecreta = Session["PalabraSecreta"] as string ?? "";
            string palabraOculta = Session["PalabraOculta"] as string ?? "";
            int intentosRestantes = (int)(Session["IntentosRestantes"] ?? 5);
            var letrasAdivinadas = Session["LetrasAdivinadas"] as List<char> ?? new List<char>();

            letra = char.ToUpperInvariant(letra);

            if (!letrasAdivinadas.Contains(letra))
            {
                letrasAdivinadas.Add(letra);
                Session["LetrasAdivinadas"] = letrasAdivinadas;

                bool acierto = false;
                var nueva = new StringBuilder(palabraOculta);
                var letraNorm = QuitarTildes(letra.ToString())[0];

                for (int i = 0; i < palabraSecreta.Length; i++)
                {
                    var normChar = QuitarTildes(palabraSecreta[i].ToString())[0];
                    if (normChar == letraNorm && nueva[i] == '_')
                    {
                        // muestra el carácter original (con acento o Ñ)
                        nueva[i] = palabraSecreta[i];
                        acierto = true;
                    }
                }

                Session["PalabraOculta"] = nueva.ToString();

                if (!acierto)
                {
                    intentosRestantes--;
                    Session["IntentosRestantes"] = intentosRestantes;

                    // (opcional) persistir fallo si el modelo tiene IntentosFallidos
                    var partidaId = (int)Session["PartidaId"];
                    var partidaDb = await db.Partidas.FindAsync(partidaId);
                    var prop = partidaDb?.GetType().GetProperty("IntentosFallidos");
                    if (prop != null && prop.CanWrite)
                    {
                        int actuales = (int)(prop.GetValue(partidaDb) ?? 0);
                        prop.SetValue(partidaDb, actuales + 1);
                        await db.SaveChangesAsync();
                    }
                }
            }

            // ¿terminó?
            if ((Session["PalabraOculta"] as string) == palabraSecreta)
            {
                // Al ganar, también mostramos la palabra
                Session["Mensaje"] = $"¡Felicidades, ganaste! La palabra era: {palabraSecreta.ToUpper()}";
                await FinalizarPartida(true);
            }
            else if (intentosRestantes <= 0)
            {
                // Al perder, mostramos cuál era la palabra
                Session["Mensaje"] = $"¡Lo siento, perdiste! La palabra era: {palabraSecreta.ToUpper()}";
                await FinalizarPartida(false);
            }

            return RedirectToAction("Jugar");
        }

        // ---------- helpers de dominio ----------

        private async Task FinalizarPartida(bool ganada)
        {
            int partidaId = (int)Session["PartidaId"];
            var partida = await db.Partidas.FindAsync(partidaId);
            if (partida != null)
            {
                var propGanada = partida.GetType().GetProperty("Ganada");
                if (propGanada != null && propGanada.CanWrite) propGanada.SetValue(partida, ganada);

                var propFechaFin = partida.GetType().GetProperty("FechaFin");
                if (propFechaFin != null && propFechaFin.CanWrite) propFechaFin.SetValue(partida, DateTime.Now);

                var jugador = await db.Jugadores.FindAsync(partida.JugadorId);
                if (jugador != null)
                {
                    if (ganada)
                    {
                        if (partida.Dificultad == Dificultad.Facil) jugador.GanadasFacil++;
                        else if (partida.Dificultad == Dificultad.Normal) jugador.GanadasNormal++;
                        else jugador.GanadasDificil++;
                    }
                    else
                    {
                        if (partida.Dificultad == Dificultad.Facil) jugador.PerdidasFacil++;
                        else if (partida.Dificultad == Dificultad.Normal) jugador.PerdidasNormal++;
                        else jugador.PerdidasDificil++;
                    }
                }
            }

            await db.SaveChangesAsync();
            Session["JuegoTerminado"] = true;
        }

        private static Dificultad ParseDificultad(string nivel)
        {
            if (string.IsNullOrWhiteSpace(nivel)) return Dificultad.Normal;
            if (Enum.TryParse(nivel.Trim(), true, out Dificultad parsed)) return parsed;

            var n = nivel.Trim().ToLowerInvariant();
            if (n.Contains("facil") || n.Contains("fácil")) return Dificultad.Facil;
            if (n.Contains("dific")) return Dificultad.Dificil;
            return Dificultad.Normal;
        }

        // Quita tildes pero respeta Ñ (para comparar)
        private static string QuitarTildes(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.Replace('ñ', '\u0001').Replace('Ñ', '\u0002');
            var form = s.Normalize(NormalizationForm.FormD);
            var arr = form.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(arr).Replace('\u0001', 'Ñ').Replace('\u0002', 'Ñ').ToUpperInvariant();
        }

        // Oculta solo letras (espacios/guiones se muestran)
        private static string InicializarOculta(string texto)
        {
            var chars = texto.Select(ch => char.IsLetter(ch) ? '_' : ch).ToArray();
            return new string(chars);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
