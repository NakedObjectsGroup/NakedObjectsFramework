// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    /*
     * These tests clear properties.  To avoid clashes with other tests, they create all the objects that they modify.
     */

    [TestClass]
    public class PropertyDelete : PropertyAbstract {
        #region Helpers

        private static string vs1 = Urls.Objects + Urls.NameSpace + @"VerySimple/1";
        private static readonly string valueProp = vs1 + Urls.Properties + "Name";
        private static readonly string refProp = vs1 + Urls.Properties + "MostSimple";

        protected override string FilePrefix {
            get { return "Property-Delete-"; }
        }

        #endregion

        [TestInitialize]
        public void CheckVerySimple1Exists() {
            try {
                Helpers.TestResponse(vs1, null);
            }
            catch (AssertFailedException) {
                var body = new JObject();
                var name = new JObject(new JProperty("value", "fred"));
                var members = new JObject(new JProperty("Name", name));
                body.Add(JsonRep.Members, members);
                body.Add(JsonRep.DomainType, Urls.NameSpace + "VerySimple");
                //Need either to suppress creation of file, or create a new method that just does the work rather than testing.
                Helpers.TestResponse(Urls.Objects, null, body.ToString(), Methods.Post, Codes.SucceededNewRepresentation);
            }


            string fred = new JObject(new JProperty("value", "fred")).ToString();
            Helpers.TestResponse(valueProp, null, fred, Methods.Put);

            string simple = Urls.Objects + Urls.MostSimple1;
            string newRef = new JObject(new JProperty("value", new JObject(new JProperty("href", simple)))).ToString();
            Helpers.TestResponse(refProp, FilePrefix + "Before", newRef, Methods.Put, Codes.Succeeded);
        }

        [TestMethod, Ignore]
        public void DeleteValueProperty() {
            Helpers.TestResponse(valueProp, FilePrefix + "After-ValueProperty", null, Methods.Delete);
        }

        [TestMethod, Ignore]
        public void DeleteReferenceProperty() {
            Helpers.TestResponse(refProp, FilePrefix + "After-ReferenceProperty", null, Methods.Delete);
        }

        [TestMethod, Ignore]
        public void AttemptDeleteNonExistentProperty() {
            Helpers.TestResponse(vs1 + Urls.Properties + "NonExistentProp", null, null, Methods.Delete, Codes.NotFound);
        }
    }
}