// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class ViewModelPut : ObjectAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "ViewModel-Put-"; }
        }

        private string key1 = "31459"; //An arbitrarily large Id

        #endregion

        [TestMethod]
        public void MostSimple() {
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 123))));
            Object(Urls.VMMostSimple + key1, "MostSimpleViewModel", body.ToString(), Methods.Put, Codes.Forbidden);
        }

        [TestMethod] 
        public void WithReference() {
            var simpleJ = new JObject(new JProperty("value", new JObject(new JProperty(JsonRep.Href, Urls.Objects + Urls.MostSimple1))));

            var body = new JObject(
                new JProperty("AReference", simpleJ),
                new JProperty("ANullReference", simpleJ),
                new JProperty("AChoicesReference", simpleJ),
                new JProperty("AnEagerReference", simpleJ),
                new JProperty("Id", new JObject(new JProperty("value", key1)))
                );
            Object(Urls.VMWithReference + "1--2--2--1", "WithReference", body.ToString(), Methods.Put, Codes.Forbidden);
        }

    }
}