using System.Web.Mvc;

namespace NakedObjects.Rest.Test.App {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
