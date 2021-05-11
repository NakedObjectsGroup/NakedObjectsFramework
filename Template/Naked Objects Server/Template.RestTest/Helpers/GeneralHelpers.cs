using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Rest.API;
using Newtonsoft.Json.Linq;
using Template.RestTest.TestCase;

namespace Template.RestTest.Helpers {
    public static class GeneralHelpers {
        public static JObject GetJObject(this JToken jToken) =>
            jToken switch {
                JObject jo => jo,
                _ => throw new AssertFailedException($"Expected JObject was {jToken.GetType()}")
            };

        public static RestfulObjectsControllerBase GetController(this AbstractRestTest tc, Methods method = Methods.Get) {
            tc.StartServerTransaction();
            var api = TestHelpers.GetController(tc).AsMethod(method);
            return api;
        }

        public static JObject GetParsedActionResult(this AbstractRestTest tc, RestfulObjectsControllerBase api, ActionResult result) {
            var (json, sc, _) = TestHelpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int) HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);
            tc.EndServerTransaction();
            return parsedResult;
        }

        public static JObject GetExtensions(this JObject jObject) =>
            jObject["extensions"].GetJObject();

        public static JObject GetResult(this JObject jObject) =>
            jObject["result"].GetJObject();

        public static string GetExtension(this JObject jObject, string name) =>
            jObject.GetExtensions()[name].ToString();

        public static string GetTitle(this JObject jObject) =>
            jObject["title"].ToString();
    }
}