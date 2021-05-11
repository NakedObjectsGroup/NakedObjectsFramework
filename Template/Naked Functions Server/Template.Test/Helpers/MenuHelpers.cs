using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Template.Test.TestCase;

namespace Template.Test.Helpers {
    public static class MenuHelpers {
        public static JObject GetMenu(this AbstractRestTest tc, string name) {
            tc.StartServerTransaction();
            var api = TestHelpers.GetController(tc);
            var result = api.GetMenu(name);
            var (json, sc, _) = TestHelpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            tc.EndServerTransaction();
            return JObject.Parse(json);
        }
    }
}