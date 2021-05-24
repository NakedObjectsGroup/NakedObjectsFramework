using Newtonsoft.Json.Linq;

namespace RestTestFramework
{
    public static class MenuHelpers {
        public static JObject GetMenu(this AbstractRestTest tc, string name) {
            var api = tc.GetController();
            var result = api.GetMenu(name);
            return tc.GetParsedActionResult(api, result);
        }
    }
}