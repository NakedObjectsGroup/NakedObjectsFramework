using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestTestFramework
{
   public static class PropertyHelpers
    {
        public static JObject AssertIsEditable(this JObject jObject)
        {
            Assert.IsNull( jObject["disabledReason"]);
            return jObject;
        }

        public static JObject AssertNotEditable(this JObject jObject)
        {
            Assert.AreEqual("Field not editable", jObject["disabledReason"].ToString());
            return jObject;
        }

        public static JObject AssertHasValue(this JObject jObject, string expected)           
        {
            Assert.AreEqual(expected, jObject["value"].ToString());
            return jObject;
        }

        public static JObject AssertHasFieldName(this JObject jObject, string expected)
        {
            Assert.AreEqual(expected, jObject["extensions"]["friendlyName"].ToString());
            return jObject;
        }
    }
}
