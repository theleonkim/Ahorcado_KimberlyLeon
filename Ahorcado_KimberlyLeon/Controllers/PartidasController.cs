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

        // POST: Partidas/Jugar → crea la partida y redirige al tablero
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
                // UTC para que el cronómetro server-side sea consistente
                FechaInicio = DateTime.UtcNow
            };

            db.Partidas.Add(partida);
            palabra.Usada = true; // si tu consigna lo pide
            await db.SaveChangesAsync();

            // Estado de juego en sesión
            Session["PartidaId"] = partida.Id;
            Session["PalabraSecreta"] = palabra.Texto;
            Session["PalabraOculta"] = InicializarOculta(palabra.Texto); // oculta solo letras
            Session["IntentosRestantes"] = 5;  // si quieres, ajusta por dificultad
            Session["LetrasAdivinadas"] = new List<char>();
            Session["JuegoTerminado"] = false;
            Session["Mensaje"] = null;

            return RedirectToAction("Jugar");   // tablero
        }

        // GET: Partidas/Jugar → tablero
        [HttpGet]
        public async Task<ActionResult> Jugar()
        {
            if (Session["PalabraSecreta"] == null)
                return RedirectToAction("Crear");

            ViewBag.PalabraOculta = Session["PalabraOculta"] as string;
            ViewBag.IntentosRestantes = Session["IntentosRestantes"];
            ViewBag.Mensaje = Session["Mensaje"];
            ViewBag.JuegoTerminado = Session["JuegoTerminado"];
            ViewBag.LetrasProbadas = string.Join(", ",
                (Session["LetrasAdivinadas"] as List<char>) ?? new List<char>());

            // segundos restantes (server-side) y penalización por rendirse (-1/-2/-3)
            int segundosRestantes = 0;
            int penalizacion = 1;

            if (Session["PartidaId"] is int pid)
            {
                var p = await db.Partidas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == pid);
                if (p != null)
                {
                    var max = Partida.GetTiempoMaximo(p.Dificultad); // 90/60/30
                    // TRATAR SIEMPRE FechaInicio como UTC (no ToUniversalTime)
                    var inicioUtc = DateTime.SpecifyKind(p.FechaInicio, DateTimeKind.Utc);
                    var deadline = inicioUtc + max;

                    // Ceil + límites para no perder 1s y no pasarse del máximo
                    segundosRestantes = (int)Math.Ceiling((deadline - DateTime.UtcNow).TotalSeconds);
                    var maxSecs = (int)max.TotalSeconds;
                    if (segundosRestantes < 0) segundosRestantes = 0;
                    if (segundosRestantes > maxSecs) segundosRestantes = maxSecs;

                    penalizacion = (p.Dificultad == Dificultad.Facil) ? 1 :
                                   (p.Dificultad == Dificultad.Normal ? 2 : 3);
                }
            }
            ViewBag.SegundosRestantes = segundosRestantes;
            ViewBag.Penalizacion = penalizacion;

            return View("Jugar"); // Views/Partidas/Jugar.cshtml
        }

        // GET: Partidas/AdivinarLetra?letra=X
        [HttpGet]
        public async Task<ActionResult> AdivinarLetra(char letra)
        {
            if (Session["JuegoTerminado"] is bool fin && fin)
                return RedirectToAction("Jugar");

            // Expiración por tiempo (server-side)
            if (Session["PartidaId"] != null)
            {
                var p = await db.Partidas.FindAsync((int)Session["PartidaId"]);
                if (p != null)
                {
                    var inicioUtc = DateTime.SpecifyKind(p.FechaInicio, DateTimeKind.Utc);
                    var deadline = inicioUtc + Partida.GetTiempoMaximo(p.Dificultad);

                    if (DateTime.UtcNow > deadline)
                    {
                        var palabraSecretaX = (Session["PalabraSecreta"] as string) ?? "";
                        Session["Mensaje"] = $"Tiempo agotado. La palabra era: {palabraSecretaX.ToUpper()}";
                        await FinalizarPartida(false);
                        return RedirectToAction("Jugar");
                    }
                }
            }

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
                    if (Session["PartidaId"] is int partidaIdPersist)
                    {
                        var partidaDb = await db.Partidas.FindAsync(partidaIdPersist);
                        var prop = partidaDb?.GetType().GetProperty("IntentosFallidos");
                        if (prop != null && prop.CanWrite)
                        {
                            int actuales = (int)(prop.GetValue(partidaDb) ?? 0);
                            prop.SetValue(partidaDb, actuales + 1);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }

            // ¿terminó?
            if ((Session["PalabraOculta"] as string) == palabraSecreta)
            {
                Session["Mensaje"] = "¡Felicidades, ganaste!";
                await FinalizarPartida(true);
            }
            else if (intentosRestantes <= 0)
            {
                Session["Mensaje"] = $"¡Lo siento, perdiste! La palabra era: {palabraSecreta.ToUpper()}";
                await FinalizarPartida(false);
            }

            return RedirectToAction("Jugar");
        }

        // POST: Partidas/Rendirse → cuenta como derrota
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Rendirse()
        {
            if (Session["JuegoTerminado"] is bool fin && fin)
                return RedirectToAction("Jugar");

            var palabraSecreta = (Session["PalabraSecreta"] as string) ?? "";
            Session["Mensaje"] = $"Te retiraste. La palabra era: {palabraSecreta.ToUpper()}";

            await FinalizarPartida(false);
            return RedirectToAction("Jugar");
        }

        // POST: Partidas/TiempoAgotado → derrota por tiempo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> TiempoAgotado()
        {
            if (Session["JuegoTerminado"] is bool fin && fin)
                return RedirectToAction("Jugar");

            var palabraSecreta = (Session["PalabraSecreta"] as string) ?? "";
            Session["Mensaje"] = $"Tiempo agotado. La palabra era: {palabraSecreta.ToUpper()}";

            await FinalizarPartida(false);
            return RedirectToAction("Jugar");
        }

        // ---------- helpers de dominio ----------

        // Cierra partida + actualiza marcador del jugador según dificultad
        private async Task FinalizarPartida(bool ganada)
        {
            int partidaId = (int)Session["PartidaId"];
            var partida = await db.Partidas.FindAsync(partidaId);
            if (partida != null)
            {
                var propGanada = partida.GetType().GetProperty("Ganada");
                if (propGanada != null && propGanada.CanWrite) propGanada.SetValue(partida, ganada);

                var propFechaFin = partida.GetType().GetProperty("FechaFin");
                if (propFechaFin != null && propFechaFin.CanWrite) propFechaFin.SetValue(partida, DateTime.UtcNow);

                var jugador = await db.Jugadores.FindAsync(partida.JugadorId);
                if (jugador != null)
                {
                    if (ganada)
                    {
                        switch (partida.Dificultad)
                        {
                            case Dificultad.Facil: jugador.GanadasFacil++; break;
                            case Dificultad.Normal: jugador.GanadasNormal++; break;
                            case Dificultad.Dificil: jugador.GanadasDificil++; break;
                        }
                    }
                    else
                    {
                        switch (partida.Dificultad)
                        {
                            case Dificultad.Facil: jugador.PerdidasFacil++; break;
                            case Dificultad.Normal: jugador.PerdidasNormal++; break;
                            case Dificultad.Dificil: jugador.PerdidasDificil++; break;
                        }
                    }
                }
            }

            await db.SaveChangesAsync();
            Session["JuegoTerminado"] = true;
        }

        private static Dificultad ParseDificultad(string nivel)
        {
            if (string.IsNullOrWhiteSpace(nivel)) return Dificultad.Normal;
            Dificultad parsed;
            if (Enum.TryParse(nivel.Trim(), true, out parsed)) return parsed;

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
