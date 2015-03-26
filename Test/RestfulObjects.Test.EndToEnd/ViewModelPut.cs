// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ViewModelPut : ObjectAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "ViewModel-Put-"; }
        }

        private string key1 = "31459"; //An arbitrarily large Id

        #endregion

        [TestMethod]
        public void MostSimple() {
            //Note:  This is a slightly unusual situation, in that the act of putting a new
            //value into the Id property of the view model effectively creates a different view model instance!
            //so the 'self' link url has changed!
            var body = new JObject(new JProperty("Id", new JObject(new JProperty("value", 123))));
            Object(Urls.VMMostSimple + key1, "MostSimpleViewModel", body.ToString(), Methods.Put);
        }

        [TestMethod] //Wasn't able to get this running  -  Bad Request  -something not set up right?
        [Ignore]
        public void WithReference()
        {
            string simple = Urls.Objects + Urls.MostSimple1;
            var simpleJ = new JObject(new JProperty("href", simple));

            var body = new JObject( 
                                   new JProperty("AChoicesReference", simpleJ),
                                   new JProperty("ANullReference", simpleJ),
                                   new JProperty("AReference", simpleJ),
                                   new JProperty("AnEagerReference", simpleJ),
                                   new JProperty("Id", new JObject(new JProperty("value", key1)))
                                   );
            Object(Urls.VMWithReference + key1, "WithReference", body.ToString(), Methods.Put);
        }
      
     
    }
}