// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class DomainTypeActionParameterDescriptionTests {
        private const string dt = Urls.DomainTypes + Urls.NameSpace + @"WithActionObject/actions/AnActionReturnsScalarWithParameters/params/parm1";
       
        [TestMethod]
        public void DomainTypeResource() {
            Helpers.TestResponse(dt, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPut() {
            Helpers.TestResponse(dt, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPost() {
            Helpers.TestResponse(dt, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptDelete() {
            Helpers.TestResponse(dt, null, null, Methods.Put, Codes.MethodNotValid);
        }
    }
}