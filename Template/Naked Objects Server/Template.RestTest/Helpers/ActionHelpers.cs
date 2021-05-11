using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Rest.API;
using NakedFramework.Rest.Model;
using Newtonsoft.Json.Linq;
using Template.RestTest.TestCase;

namespace Template.RestTest.Helpers {
    public static class ActionHelpers {
        public static JObject InvokeAction(this AbstractRestTest tc, Key key, string actionName, Methods method = Methods.Get) => tc.InvokeAction(key, actionName, new List<(string name, object value)>(), method);

        public static JObject InvokeAction(this AbstractRestTest tc, string menuName, string actionName, Methods method = Methods.Get) => tc.InvokeAction(menuName, actionName, new List<(string name, object value)>(), method);

        private static IValue GetReferenceValue(JObject rawValue) {
            var href = rawValue.GetHref();
            var typeName = rawValue.GetDomainType();
            return new ReferenceValue(href, typeName);
        }

        private static IValue GetScalarValue(object rawValue) => new ScalarValue(rawValue);

        private static IValue GetIValue(object rawValue) {
            return rawValue switch {
                JObject jo => GetReferenceValue(jo),
                { } v => GetScalarValue(v),
                null => throw new AssertFailedException("Unexpected null parameter")
            };
        }

        private static (RestfulObjectsControllerBase api, ArgumentMap map) SetupForInvoke(this AbstractRestTest tc, IList<(string name, object value)> parameters, Methods method = Methods.Get) {
            var api = tc.GetController(method);
            var parameterDictionary = parameters.ToDictionary(t => t.name, t => GetIValue(t.value));
            var map = new ArgumentMap {Map = parameterDictionary};
            return (api, map);
        }

        private static ActionResult Invoke(this RestfulObjectsControllerBase api, Key key, string actionName, ArgumentMap map, Methods method) =>
            method switch {
                Methods.Post => api.PostInvoke(key.Type, key.Id, actionName, map),
                Methods.Get => api.GetInvoke(key.Type, key.Id, actionName, map),
                Methods.Put => api.PutInvoke(key.Type, key.Id, actionName, map),
                _ => throw new AssertFailedException($"Unexpected method {method}")
            };

        private static ActionResult Invoke(this RestfulObjectsControllerBase api, string menuName, string actionName, ArgumentMap map, Methods method) =>
            method switch {
                Methods.Post => api.PostInvokeOnMenu(menuName, actionName, map),
                Methods.Get => api.GetInvokeOnMenu(menuName, actionName, map),
                Methods.Put => api.PutInvokeOnMenu(menuName, actionName, map),
                _ => throw new AssertFailedException($"Unexpected method {method}")
            };

        public static JObject InvokeAction(this AbstractRestTest tc, Key key, string actionName, IList<(string name, object value)> parameters, Methods method = Methods.Get) {
            var (api, map) = tc.SetupForInvoke(parameters, method);
            var result = api.Invoke(key, actionName, map, method);
            return tc.GetParsedActionResult(api, result).GetResult();
        }

        public static JObject InvokeAction(this AbstractRestTest tc, string menuName, string actionName, IList<(string name, object value)> parameters, Methods method = Methods.Get) {
            var (api, map) = tc.SetupForInvoke(parameters, method);
            var result = api.Invoke(menuName, actionName, map, method);
            return tc.GetParsedActionResult(api, result).GetResult();
        }

        public static IList<(string name, object value)> CreateParameters(params (string, object)[] param) => param.ToList();

        public static string GetValue(this JObject jObject) => jObject["value"].ToString();
    }
}