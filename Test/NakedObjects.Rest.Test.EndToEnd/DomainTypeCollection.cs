// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class DomainTypeCollectionTests {
        private const string dtc = Urls.DomainTypes + Urls.NameSpace + @"WithCollection/collections/AnEmptyCollection";
        [TestMethod]
        public void DomainTypeCollection() {
            Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPut() {
            Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPost() {
            Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptDelete() {
            Helpers.TestResponse(dtc, null, null, Methods.Put, Codes.MethodNotValid);
        }
    }
}