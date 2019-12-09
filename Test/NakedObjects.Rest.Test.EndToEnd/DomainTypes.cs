// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ZZDomainTypesTests { // make run last so all types are in memory
        [TestMethod]
        
        public void DomainTypes() {
            Helpers.TestResponse(Urls.DomainTypes, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPut() {
            Helpers.TestResponse(Urls.DomainTypes, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPost() {
            Helpers.TestResponse(Urls.DomainTypes, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptDelete() {
            Helpers.TestResponse(Urls.DomainTypes, null, null, Methods.Put, Codes.MethodNotValid);
        }
    }
}