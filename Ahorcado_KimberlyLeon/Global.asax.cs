using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Ahorcado_KimberlyLeon.Data;        // <- tu DbContext
using Ahorcado_KimberlyLeon.Migrations;  // <- clase Configuration de Migrations

namespace Ahorcado_KimberlyLeon
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Aplica automáticamente las migraciones pendientes al arrancar
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<AhorcadoContext, Configuration>()
            );

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
