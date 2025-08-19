using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            ViewBag.JugadorId = new SelectList(await db.Jugadores.ToListAsync(), "Id", "Nombre");
            return View();
        }

        // POST: Partidas/Jugar  (antes le llamabas Crear -> ahora Jugar crea la partida)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Jugar(int JugadorId, string Nivel)
        {
            // 1) Validar jugador
            var jugador = await db.Jugadores.FindAsync(JugadorId);
            if (jugador == null)
            {
                return HttpNotFound();
            }

            // 2) Tomar una palabra disponible al azar
            var palabra = await db.Palabras
                                  .Where(p => !p.Usada)
                                  .OrderBy(p => Guid.NewGuid())
                                  .FirstOrDefaultAsync();

            if (palabra == null)
            {
                ViewBag.MensajeError = "No hay más palabras disponibles en el diccionario.";
                ViewBag.JugadorId = new SelectList(await db.Jugadores.ToListAsync(), "Id", "Nombre");
                return View("Crear");
            }

            // 3) Parsear nivel (string -> enum Dificultad)
            var dificultad = ParseDificultad(Nivel); // Facil/Normal/Dificil (default: Normal)

            // 4) Crear partida (usa enum; NO asignes strings a Dificultad)
            var nuevaPartida = new Partida
            {
                JugadorId = jugador.Id,
                PalabraId = palabra.Id,
                Dificultad = dificultad,
                FechaInicio = DateTime.Now,
                // Las siguientes propiedades existen si tu modelo las tiene; si no, no pasa nada por omitirlas:
                // IntentosFallidos = 0,
                // Ganada = false,
                // Estado = EstadoPartida.EnCurso
            };

            db.Partidas.Add(nuevaPartida);

            // Marcar palabra como usada (según tu requerimiento)
            palabra.Usada = true;

            await db.SaveChangesAsync();

            // 5) Estado de sesión para la mecánica de UI
            Session["PartidaId"] = nuevaPartida.Id;
            Session["PalabraSecreta"] = palabra.Texto; // ajusta si tu propiedad se llama distinto
            Session["PalabraOculta"] = new string('_', palabra.Texto.Length);
            Session["IntentosRestantes"] = 5; // el enunciado pide 5 fallos máx
            Session["LetrasAdivinadas"] = new List<char>();
            Session["JuegoTerminado"] = false;
            Session["Mensaje"] = null;

            return RedirectToAction("Juego");
        }

        // GET: Partidas/Juego
        public ActionResult Juego()
        {
            if (Session["PalabraSecreta"] == null)
                return RedirectToAction("Crear");

            ViewBag.PalabraOculta = Session["PalabraOculta"] as string;
            ViewBag.IntentosRestantes = Session["IntentosRestantes"];
            ViewBag.Mensaje = Session["Mensaje"];
            ViewBag.JuegoTerminado = Session["JuegoTerminado"];

            return View();
        }

        // POST: Partidas/AdivinarLetra
        [HttpPost]
        public async Task<ActionResult> AdivinarLetra(char letra)
        {
            if (Session["JuegoTerminado"] is bool terminado && terminado)
                return RedirectToAction("Juego");

            string palabraSecreta = Session["PalabraSecreta"] as string;
            string palabraOculta = Session["PalabraOculta"] as string;
            int intentosRestantes = (int)Session["IntentosRestantes"];
            var letrasAdivinadas = Session["LetrasAdivinadas"] as List<char> ?? new List<char>();

            letra = char.ToLowerInvariant(letra);

            if (!letrasAdivinadas.Contains(letra))
            {
                letrasAdivinadas.Add(letra);
                Session["LetrasAdivinadas"] = letrasAdivinadas;

                bool letraEncontrada = false;
                var nuevaPalabraOculta = new StringBuilder(palabraOculta);

                for (int i = 0; i < palabraSecreta.Length; i++)
                {
                    if (char.ToLowerInvariant(palabraSecreta[i]) == letra)
                    {
                        nuevaPalabraOculta[i] = palabraSecreta[i];
                        letraEncontrada = true;
                    }
                }

                Session["PalabraOculta"] = nuevaPalabraOculta.ToString();

                if (!letraEncontrada)
                {
                    intentosRestantes--;
                    Session["IntentosRestantes"] = intentosRestantes;

                    // Opcional: si tu modelo Partida tiene IntentosFallidos, persiste el fallo.
                    var partidaId = (int)Session["PartidaId"];
                    var partidaDb = await db.Partidas.FindAsync(partidaId);
                    if (partidaDb != null)
                    {
                        // Si tienes IntentosFallidos en el modelo:
                        var prop = partidaDb.GetType().GetProperty("IntentosFallidos");
                        if (prop != null && prop.CanWrite)
                        {
                            int actuales = (int)(prop.GetValue(partidaDb) ?? 0);
                            prop.SetValue(partidaDb, actuales + 1);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }

            // ¿Ganó?
            if ((Session["PalabraOculta"] as string) == palabraSecreta)
            {
                Session["Mensaje"] = "¡Felicidades, ganaste!";
                await FinalizarPartida(true);
            }
            else if (intentosRestantes <= 0)
            {
                Session["Mensaje"] = "¡Lo siento, perdiste!";
                await FinalizarPartida(false);
            }

            return RedirectToAction("Juego");
        }

        private async Task FinalizarPartida(bool ganada)
        {
            int partidaId = (int)Session["PartidaId"];
            var partida = await db.Partidas.FindAsync(partidaId);
            if (partida != null)
            {
                // Estas propiedades existen si tu modelo las tiene. Si no, comenta las que falten.
                var propGanada = partida.GetType().GetProperty("Ganada");
                if (propGanada != null && propGanada.CanWrite) propGanada.SetValue(partida, ganada);

                var propFechaFin = partida.GetType().GetProperty("FechaFin");
                if (propFechaFin != null && propFechaFin.CanWrite) propFechaFin.SetValue(partida, DateTime.Now);

                // Actualizar estadísticas del jugador por dificultad (enum)
                var jugador = await db.Jugadores.FindAsync(partida.JugadorId);
                if (jugador != null)
                {
                    // Si tu modelo Jugador tiene estas propiedades:
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

        // ----------------- helpers -----------------

        private static Dificultad ParseDificultad(string nivel)
        {
            if (string.IsNullOrWhiteSpace(nivel)) return Dificultad.Normal;

            // intento directo (case-insensitive)
            if (Enum.TryParse(nivel.Trim(), true, out Dificultad parsed))
                return parsed;

            // Fallback por palabras clave
            var n = (nivel ?? "").Trim().ToLowerInvariant();
            if (n.Contains("facil")) return Dificultad.Facil;
            if (n.Contains("dific")) return Dificultad.Dificil;
            return Dificultad.Normal;
        }
    }
}
