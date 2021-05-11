using Newtonsoft.Json.Linq;
using Template.RestTest.TestCase;

namespace Template.RestTest.Helpers {
    public static class MenuHelpers {
        public static JObject GetMenu(this AbstractRestTest tc, string name) {
            var api = tc.GetController();
            var result = api.GetMenu(name);
            return tc.GetParsedActionResult(api, result);
        }
    }
}