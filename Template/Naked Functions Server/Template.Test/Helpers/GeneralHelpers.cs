using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Template.Test.Helpers {
    public static class GeneralHelpers {
        public static JObject GetJObject(this JToken jToken) =>
            jToken switch {
                JObject jo => jo,
                _ => throw new AssertFailedException($"Expected JObject was {jToken.GetType()}")
            };

        public static JObject GetExtensions(this JObject jObject) =>
            jObject["extensions"].GetJObject();

        public static JObject GetResult(this JObject jObject) =>
            jObject["result"].GetJObject();

        public static string GetExtension(this JObject jObject, string name) =>
            jObject.GetExtensions()[name].ToString();
    }
}