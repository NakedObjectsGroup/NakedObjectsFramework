// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public abstract class CollectionAbstract {
        private static string Collections = Urls.Objects + Urls.WithCollection1 + Urls.Collections;
        protected static string vs1 = Urls.Objects + Urls.NameSpace + @"VerySimple/1";
        protected static string simpleSet = vs1 + Urls.Collections + "SimpleSet";
        protected static string simpleList = vs1 + Urls.Collections + "SimpleList";

        protected JObject simple1AsArgument = new JObject(new JProperty("value", new JObject(JsonRep.MostSimple1AsRef())));
        protected JObject simple2AsArgument = new JObject(new JProperty("value", new JObject(JsonRep.MostSimple2AsRef())));
        protected JObject simple3AsArgument = new JObject(new JProperty("value", new JObject(JsonRep.MostSimple3AsRef())));
        protected JObject withValueAsArgument = new JObject(new JProperty("value", new JObject(JsonRep.WithValue1AsRef())));

        protected virtual string FilePrefix {
            get { return "NO PREFIX SPECIFIED"; }
        }

        protected void Collection(string collectionName, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded, string acceptHeader = null) {
            Helpers.TestResponse(Collections + collectionName, FilePrefix + fileName, body, method, code, acceptHeader);
        }


        [TestInitialize]
        public void InitializeVerySimple1() {
            if (! Helpers.UrlExistsForGet(vs1)) {
                var body = new JObject();
                var name = new JObject(new JProperty("value", "fred"));
                var members = new JObject(new JProperty("Name", name));
                body.Add(JsonRep.Members, members);
                body.Add(JsonRep.DomainType, Urls.NameSpace + "VerySimple");
                //Need either to suppress creation of file, or create a new method that just does the work rather than testing.
                Helpers.TestResponse(Urls.Objects, null, body.ToString(), Methods.Post, Codes.SucceededNewRepresentation);
            }

            //Clear both the collections first
            string url = vs1 + Urls.Actions + "EmptyTheSet" + Urls.Invoke;
            Helpers.TestResponse(url, null, null, Methods.Post);
            url = vs1 + Urls.Actions + "EmptyTheList" + Urls.Invoke;
            Helpers.TestResponse(url, null, null, Methods.Post);
        }
    }
}