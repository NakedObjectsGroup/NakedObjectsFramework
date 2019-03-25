// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    // for the moment ignore these
    [TestClass] // These tests are corrupting MostSimple/1 and hence causing other tests to fail
    public class ProtoPersistentObjects {
        #region Helpers

        private static JObject ProtoPersistentMostSimple(int val) {
            var body = new JObject();
            var id = new JObject(new JProperty("value", val));
            var members = new JObject(new JProperty("Id", id));
            body.Add(JsonRep.Members, members);
            return body;
        }

        private static JObject ProtoPersistentVerySimple(int val) {
            var body = new JObject();
            var id = new JProperty("Id", new JObject(new JProperty("value", val)));
            var ms1 = new JProperty("MostSimple", new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1)))));
            var name = new JProperty("Name", new JObject(new JProperty("value", "fred")));
            var members = new JObject(ms1, name, id);
            body.Add(JsonRep.Members, members);
            return body;
        }

        private static JObject ProtoPersistentMalformed(int val) {
            var body = new JObject();
            var id = new JObject(new JProperty("value", val));
            var members = new JObject(new JProperty("id", id)); //Because 'Id' should be capitalised
            body.Add(JsonRep.Members, members);
            return body;
        }

        #endregion

        [TestMethod]
        public void CreateTransientMostSimple() {
            string url = Urls.RestDataRepository + Urls.Actions + @"CreateTransientMostSimple/" + Urls.Invoke;
            Helpers.TestResponse(url, "CreateTransientMostSimple", null, Methods.Post);
        }

        [TestMethod]
        public void CreateTransientWithValue() {
            string url = Urls.RestDataRepository + Urls.Actions + @"CreateTransientWithValue/" + Urls.Invoke;
            Helpers.TestResponse(url, "CreateTransientWithValue", null, Methods.Post);
        }

        [TestMethod]
        public void CreateTransientWithReference() {
            string url = Urls.RestDataRepository + Urls.Actions + @"CreateTransientWithReference/" + Urls.Invoke;
            Helpers.TestResponse(url, "CreateTransientWithReference", null, Methods.Post);
        }

        [TestMethod]
        public void CreateTransientWithCollection() {
            string url = Urls.RestDataRepository + Urls.Actions + @"CreateTransientWithCollection/" + Urls.Invoke;
            Helpers.TestResponse(url, "CreateTransientWithCollection", null, Methods.Post);
        }

        [TestMethod]
        public void PersistMostSimple() {
            string body = ProtoPersistentMostSimple(10001).ToString();
            Helpers.TestResponse(Urls.Objects + Urls.NameSpace + "MostSimplePersist", "PersistMostSimple", body, Methods.Post, Codes.SucceededNewRepresentation);
        }

        [TestMethod]
        [Ignore] // temp ignore 
        public void PersistVerySimple() {
            string body = ProtoPersistentVerySimple(10002).ToString();
            Helpers.TestResponse(Urls.Objects + Urls.NameSpace + "VerySimplePersist", "PersistVerySimple", body, Methods.Post, Codes.SucceededNewRepresentation);
        }

        [TestMethod]
        public void AttemptPersistWithPut() {
            string body = ProtoPersistentMostSimple(10003).ToString();
            Helpers.TestResponse(Urls.Objects + Urls.NameSpace + "MostSimplePersist", null, body, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPersistWithMalformedBody() {
            string body = ProtoPersistentMalformed(10004).ToString();
            Helpers.TestResponse(Urls.Objects + Urls.NameSpace + "MostSimplePersist", null, body, Methods.Post, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPersistWithUnknownType() {
            string body = ProtoPersistentMostSimple(10005).ToString();
            Helpers.TestResponse(Urls.Objects + Urls.NameSpace + "NotAType", null, body, Methods.Post, Codes.NotFound);
        }
    }
}