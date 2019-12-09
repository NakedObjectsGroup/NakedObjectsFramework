// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class DomainTypeActionInvokeTests {
        private static string withActionObjectTypeActions = Urls.DomainTypes + Urls.NameSpace + @"WithActionObject" + Urls.TypeActions;
        private static string withActionTypeActions = Urls.DomainTypes + Urls.NameSpace + @"WithAction" + Urls.TypeActions;

        private static string isSubtypeOf = withActionObjectTypeActions + @"isSubtypeOf/invoke?supertype=" + Urls.NameSpace;
        private static string isSupertypeOf = withActionTypeActions + @"isSupertypeOf/invoke?subtype=" + Urls.NameSpace;

        [TestMethod]
        public void IsSubtypeOf() {
            Helpers.TestResponse(isSubtypeOf + "WithAction", "IsSubtypeOf");
        }

        [TestMethod]
        public void IsSubtypeOfFalse() {
            Helpers.TestResponse(isSubtypeOf + "MostSimple", "IsSubtypeOfFalse");
        }

        [TestMethod]
        public void IsSubtypeOfNonExistentType() {
            Helpers.TestResponse(isSubtypeOf + "NoSuchClass", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void IsSubtypeOfMalformed() {
            string url = withActionObjectTypeActions + @"isSubtypeOf/invoke?subtype=" + Urls.NameSpace + "MostSimple";
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void IsSubtypeOfNoArgs() {
            string url = withActionObjectTypeActions + @"isSubtypeOf/invoke";
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void IsSupertypeOf() {
            Helpers.TestResponse(isSupertypeOf + @"WithActionObject", "IsSupertypeOf");
        }

        [TestMethod]
        public void IsSupertypeOfFalse() {
            Helpers.TestResponse(isSupertypeOf + @"MostSimple", "IsSupertypeOfFalse");
        }

        [TestMethod]
        public void IsSupertypeOfNonExistentType() {
            Helpers.TestResponse(isSupertypeOf + "NoSuchClass", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void IsSupertypeOfMalformed() {
            string url = withActionObjectTypeActions + @"isSupertypeOf/invoke?supertype=" + Urls.NameSpace + "WithActionObject";
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void IsSupertypeOfNoArgs() {
            string url = withActionObjectTypeActions + @"isSupertypeOf/invoke";
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void AttemptPut() {
            Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
            Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptPost() {
            Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
            Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        }

        [TestMethod]
        public void AttemptDelete() {
            Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
            Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        }
    }
}