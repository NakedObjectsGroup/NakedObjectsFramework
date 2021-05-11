using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Rest.Model;
using Newtonsoft.Json.Linq;
using Template.Test.TestCase;

namespace Template.Test.Helpers {
    public static class ActionHelpers {
        public static JObject InvokeAction(this ITestCase tc, Key key, string actionName) => tc.InvokeAction(key, actionName, new List<(string name, string value)>());

        public static JObject InvokeAction(this ITestCase tc, Key key, string actionName, IList<(string name, string value)> parameters)
        {
            tc.StartServerTransaction();
            var api = TestHelpers.Api(tc).AsPost();
            var parameterDictionary = parameters.ToDictionary(t => t.name, t => (IValue) new ScalarValue(t.value));
            var map = new ArgumentMap { Map = parameterDictionary };

            var result = api.PostInvoke(key.Type, key.Id, actionName, map);
            var (json, sc, _) = TestHelpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);
            tc.EndServerTransaction();

            return parsedResult.GetResult();
        }

        public static IList<(string name, string value)> CreateParameters(params (string, string)[] param) => param.ToList();

        public static string GetValue(this JObject jObject) =>
            jObject["value"].ToString();
    }
}