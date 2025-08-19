using System.Web;
using System.Web.Mvc;

namespace Ahorcado_KimberlyLeón
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
