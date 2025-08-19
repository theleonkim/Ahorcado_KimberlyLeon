using System.Web.Optimization;

namespace Ahorcado_KimberlyLeon
{
    public class BundleConfig
    {

        public static void RegisterBundles(BundleCollection bundles)
        {
            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // jQuery Validate (y unobtrusive)
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Modernizr (usar versión de dev; para prod, construir a medida en modernizr.com)
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            // Bootstrap (JS)  -> OJO: ScriptBundle, no Bundle
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js"
                        // Si estás en Bootstrap 3.x, puedes agregar respond.js si lo tienes:
                        // ,"~/Scripts/respond.js"
                        ));

            // CSS (Bootstrap + tu site.css)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/site.css"
                        // Si agregaste un css propio (ej. ahorcado.css), inclúyelo aquí:
                        // ,"~/Content/ahorcado.css"
                        ));

            // Desactivar la optimización para depurar el error (o en modo Debug)
            // Cuando resuelvas el problema, puedes quitar esta línea para volver a activar la minificación.
            BundleTable.EnableOptimizations = false;
        }
    }
}