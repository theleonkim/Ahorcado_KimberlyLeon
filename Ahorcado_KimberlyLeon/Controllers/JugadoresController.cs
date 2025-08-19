using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net;

namespace Ahorcado_KimberlyLeon.Controllers
{
    public class JugadoresController : Controller
    {
        private AhorcadoContext db = new AhorcadoContext();

        // GET: Jugadores/Crear
        public ActionResult Crear()
        {
            return View();
        }

        // POST: Jugadores/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear([Bind(Include = "Id,Nombre")] Jugador jugador)
        {
            if (ModelState.IsValid)
            {
                var existeJugador = await db.Jugadores.AnyAsync(j => j.Id == jugador.Id);
                if (existeJugador)
                {
                    ModelState.AddModelError("Id", "La identificación ya existe.");
                    return View(jugador);
                }

                db.Jugadores.Add(jugador);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(jugador);
        }

        // GET: Jugadores/Escalafon
        public async Task<ActionResult> Escalafon()
        {
            var jugadores = await db.Jugadores
                .OrderByDescending(j => j.Marcador)
                .ToListAsync();

            return View(jugadores);
        }
    }
}