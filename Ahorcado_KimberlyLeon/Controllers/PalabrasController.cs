using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ahorcado_KimberlyLeon.Controllers
{
    public class PalabrasController : Controller
    {
        private readonly AhorcadoContext db = new AhorcadoContext();

        // GET: /Palabras
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var palabras = await db.Palabras
                                   .OrderBy(p => p.Texto)
                                   .ToListAsync();
            return View(palabras);
        }

        // GET: /Palabras/Crear
        [HttpGet]
        public ActionResult Crear() => View();

        // POST: /Palabras/Crear   <-- ESTO ES LO QUE TE FALTABA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(Palabra palabra)
        {
            if (!ModelState.IsValid)
                return View(palabra);

            if (palabra == null || string.IsNullOrWhiteSpace(palabra.Texto))
            {
                ModelState.AddModelError("", "Ingrese una palabra.");
                return View(palabra);
            }

            // Normaliza para evitar duplicados con/ sin tilde y mayúsculas
            var textoNorm = QuitarTildes(palabra.Texto).ToLowerInvariant();

            // EF6 no traduce QuitarTildes a SQL → materializa y compara en memoria
            var existentes = await db.Palabras.Select(p => p.Texto).ToListAsync();
            var yaExiste = existentes.Any(t => QuitarTildes(t).ToLowerInvariant() == textoNorm);

            if (yaExiste)
            {
                ModelState.AddModelError("", "La palabra ya existe (ignorando tildes y mayúsculas).");
                return View(palabra);
            }

            // Si tu entidad Palabra tiene 'Usada' (bool)
            var propUsada = typeof(Palabra).GetProperty("Usada");
            if (propUsada != null && propUsada.PropertyType == typeof(bool) && propUsada.CanWrite)
                propUsada.SetValue(palabra, false);

            db.Palabras.Add(palabra);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ---- helpers ----
        private static string QuitarTildes(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var d = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(d.Length);
            foreach (var c in d)
            {
                var cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
