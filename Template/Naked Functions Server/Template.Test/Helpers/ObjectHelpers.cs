using Newtonsoft.Json.Linq;
using Template.Test.TestCase;

namespace Template.Test.Helpers {
    public static class ObjectHelpers {
        public static JObject GetObject(this AbstractRestTest tc, Key key) {
            var api = tc.GetController();
            var result = api.GetObject(key.Type, key.Id);
            return tc.GetParsedActionResult(api, result);
        }

        public static JObject GetMembers(this JObject jObject) =>
            jObject["members"].GetJObject();

        public static JObject GetMember(this JObject jObject, string name) =>
            jObject.GetMembers()[name].GetJObject();

        public static string GetHref(this JObject jObject) =>
            jObject["links"][0]["href"].ToString();

        public static string GetDomainType(this JObject jObject) =>
            jObject["domainType"].ToString();
    }
}