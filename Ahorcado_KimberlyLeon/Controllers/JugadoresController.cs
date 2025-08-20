using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;

namespace Ahorcado_KimberlyLeon.Controllers
{
    public class JugadoresController : Controller
    {
        private readonly AhorcadoContext db = new AhorcadoContext();

        // (Opcional) Listado simple
        public async Task<ActionResult> Index()
        {
            var jugadores = await db.Jugadores.OrderBy(j => j.Nombre).ToListAsync();
            return View(jugadores);
        }

        // GET: Jugadores/Crear
        [HttpGet]
        public ActionResult Crear() => View(new Jugador());

        // POST: Jugadores/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(Jugador model)
        {
            if (!ModelState.IsValid) return View(model);

            var nombre = (model.Nombre ?? "").Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                ModelState.AddModelError("Nombre", "Ingrese un nombre.");
                return View(model);
            }

            bool existe = await db.Jugadores
                                  .AnyAsync(j => j.Nombre.ToLower() == nombre.ToLower());
            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe un jugador con ese nombre.");
                return View(model);
            }

            model.Nombre = nombre;
            db.Jugadores.Add(model);
            await db.SaveChangesAsync();

            // <<< el toast del layout lee este TempData
            TempData["Ok"] = "Jugador guardado con éxito.";
            return RedirectToAction("Escalafon");
        }

        // GET: Jugadores/Escalafon
        public async Task<ActionResult> Escalafon()
        {
            // Calculamos "Marcador" en la consulta (EF6 lo soporta)
            var lista = await db.Jugadores
                .Select(j => new JugadorEscalafonVM
                {
                    Id = j.Id,
                    Nombre = j.Nombre,
                    GanadasFacil = j.GanadasFacil,
                    GanadasNormal = j.GanadasNormal,
                    GanadasDificil = j.GanadasDificil,
                    PerdidasFacil = j.PerdidasFacil,
                    PerdidasNormal = j.PerdidasNormal,
                    PerdidasDificil = j.PerdidasDificil,
                    Marcador = (j.GanadasFacil + j.GanadasNormal + j.GanadasDificil)
                             - (j.PerdidasFacil + j.PerdidasNormal + j.PerdidasDificil)
                })
                .OrderByDescending(j => j.Marcador)
                .ThenBy(j => j.Nombre)
                .ToListAsync();

            return View(lista);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
