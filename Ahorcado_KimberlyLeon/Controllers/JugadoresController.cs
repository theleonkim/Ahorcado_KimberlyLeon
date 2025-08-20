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
            var codigo = (model.Codigo ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nombre))
                ModelState.AddModelError("Nombre", "Ingrese un nombre.");
            if (string.IsNullOrWhiteSpace(codigo))
                ModelState.AddModelError("Codigo", "Ingrese el ID del jugador.");

            if (!ModelState.IsValid) return View(model);

            // Unicidad por Código (el ID que escribe la persona)
            bool codigoOcupado = await db.Jugadores.AnyAsync(j => j.Codigo == codigo);
            if (codigoOcupado)
            {
                ModelState.AddModelError("Codigo", "Ese ID ya existe.");
                return View(model);
            }

            // (Opcional) evita nombres duplicados case-insensitive
            bool nombreOcupado = await db.Jugadores.AnyAsync(j => j.Nombre.ToLower() == nombre.ToLower());
            if (nombreOcupado)
            {
                ModelState.AddModelError("Nombre", "Ya existe un jugador con ese nombre.");
                return View(model);
            }

            model.Nombre = nombre;
            model.Codigo = codigo;

            db.Jugadores.Add(model);
            await db.SaveChangesAsync();

            TempData["Ok"] = "Jugador guardado con éxito.";
            return RedirectToAction("Escalafon");
        }


        // GET: Jugadores/Escalafon
        // GET: Jugadores/Escalafon
        // GET: Jugadores/Escalafon
        public async Task<ActionResult> Escalafon()
        {
            var lista = await db.Jugadores
                .Select(j => new JugadorEscalafonVM
                {
                    Id = j.Id,
                    Codigo = j.Codigo, // <<< mostrarás este
                    Nombre = j.Nombre,
                    GanadasFacil = j.GanadasFacil,
                    GanadasNormal = j.GanadasNormal,
                    GanadasDificil = j.GanadasDificil,
                    PerdidasFacil = j.PerdidasFacil,
                    PerdidasNormal = j.PerdidasNormal,
                    PerdidasDificil = j.PerdidasDificil,
                    Marcador = (j.GanadasFacil * 1 + j.GanadasNormal * 2 + j.GanadasDificil * 3)
                             - (j.PerdidasFacil * 1 + j.PerdidasNormal * 2 + j.PerdidasDificil * 3)
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
