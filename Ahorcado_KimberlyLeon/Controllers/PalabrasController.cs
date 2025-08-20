using Ahorcado_KimberlyLeon.Data;
using Ahorcado_KimberlyLeon.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Ahorcado_KimberlyLeon.Controllers
{
    public class PalabrasController : Controller
    {
        private readonly AhorcadoContext db = new AhorcadoContext();

        // GET: Palabras
        public async Task<ActionResult> Index()
        {
            var palabras = await db.Palabras
                                   .OrderBy(p => p.Id)
                                   .ToListAsync();
            return View(palabras);
        }

        // GET: Palabras/Crear
        public ActionResult Crear()
        {
            // vista con un textbox para Texto
            return View();
        }

        // POST: Palabras/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear([Bind(Include = "Texto")] Palabra modelo)
        {
            // Normalización y validaciones mínimas
            if (modelo == null) modelo = new Palabra();

            if (string.IsNullOrWhiteSpace(modelo.Texto))
                ModelState.AddModelError("Texto", "Ingresa una palabra.");

            var texto = (modelo.Texto ?? string.Empty).Trim();

            // Duplicados (case-insensitive)
            bool existe = await db.Palabras
                                  .AnyAsync(p => p.Texto.ToLower() == texto.ToLower());
            if (existe)
                ModelState.AddModelError("Texto", "La palabra ya existe en el diccionario.");

            if (!ModelState.IsValid)
                return View(modelo);

            var entidad = new Palabra
            {
                Texto = texto,
                Usada = false
            };

            db.Palabras.Add(entidad);
            await db.SaveChangesAsync();

            // Lanzamos toast / flash para el layout
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Palabra guardada con éxito.";
            TempData["FlashSuccess"] = "Palabra guardada con éxito.";

            return RedirectToAction("Index");
        }

        // GET: Palabras/Editar/5
        public async Task<ActionResult> Editar(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var palabra = await db.Palabras.FindAsync(id.Value);
            if (palabra == null) return HttpNotFound();

            return View(palabra);
        }

        // POST: Palabras/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar([Bind(Include = "Id,Texto,Usada")] Palabra modelo)
        {
            if (modelo == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(modelo.Texto))
                ModelState.AddModelError("Texto", "Ingresa una palabra.");

            var texto = (modelo.Texto ?? string.Empty).Trim();

            bool duplicada = await db.Palabras
                                     .AnyAsync(p => p.Id != modelo.Id &&
                                                    p.Texto.ToLower() == texto.ToLower());
            if (duplicada)
                ModelState.AddModelError("Texto", "Ya existe otra palabra con ese texto.");

            if (!ModelState.IsValid)
                return View(modelo);

            var entidad = await db.Palabras.FindAsync(modelo.Id);
            if (entidad == null) return HttpNotFound();

            entidad.Texto = texto;
            entidad.Usada = modelo.Usada; // respeta el valor actual

            await db.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Palabra actualizada.";
            TempData["FlashSuccess"] = "Palabra actualizada.";

            return RedirectToAction("Index");
        }

        // POST: Palabras/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(int id)
        {
            var entidad = await db.Palabras.FindAsync(id);
            if (entidad == null) return HttpNotFound();

            try
            {
                db.Palabras.Remove(entidad);
                await db.SaveChangesAsync();

                TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Palabra eliminada.";
                TempData["FlashSuccess"] = "Palabra eliminada.";
            }
            catch (Exception)
            {
                // Si hay FKs (Partidas) puede fallar
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "No se pudo eliminar. Puede estar en uso.";
                TempData["FlashError"] = "No se pudo eliminar. Puede estar en uso.";
            }

            return RedirectToAction("Index");
        }

        // OPCIONAL: Reiniciar el flag Usada para todas las palabras
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReiniciarUso()
        {
            var todas = await db.Palabras.ToListAsync();
            foreach (var p in todas) p.Usada = false;
            await db.SaveChangesAsync();

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Se reinició el estado de uso de todas las palabras.";
            TempData["FlashSuccess"] = "Se reinició el estado de uso de todas las palabras.";

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
