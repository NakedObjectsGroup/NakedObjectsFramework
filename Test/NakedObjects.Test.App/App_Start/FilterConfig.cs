using System.Web;
using System.Web.Mvc;

namespace NakedObjects.Mvc.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
