using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Template.Test.TestCase;

namespace Template.Test.Helpers {
    public static class ObjectHelpers {
        public static JObject GetObject(this AbstractRestTest tc, Key key) {
            tc.StartServerTransaction();
            var api = TestHelpers.GetController(tc);
            var result = api.GetObject(key.Type, key.Id);
            var (json, sc, _) = TestHelpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            tc.EndServerTransaction();
            return JObject.Parse(json);
        }

        public static JObject GetMembers(this JObject jObject) =>
            jObject["members"].GetJObject();

        public static JObject GetMember(this JObject jObject, string name) =>
            jObject.GetMembers()[name].GetJObject();

        public static string GetTitle(this JObject jObject) =>
            jObject["title"].ToString();
    }
}