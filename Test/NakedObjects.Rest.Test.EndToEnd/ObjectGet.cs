// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ObjectGet : ObjectAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Object-Get-"; }
        }

        #endregion

        [TestMethod]
        public void MostSimple() {
            Object(Urls.MostSimple1, "MostSimple");
        }

        [TestMethod]
        public void WithFormalDomainModel() {
            Object(Urls.MostSimple1 + JsonRep.FormalDomainModeAsQueryString, "WithFormalDomainModel");
        }

        [TestMethod]
        public void WithSimpleDomainModel() {
            Object(Urls.MostSimple1 + JsonRep.SimpleDomainModeAsQueryString, "WithSimpleDomainModel");
        }

        [TestMethod] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            string url = Urls.Objects + Urls.MostSimple1 + JsonRep.DomainModeQueryStringMalformed;
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        [TestMethod]
        public void WithGenericAcceptHeader() {
            Object(Urls.MostSimple1, "MostSimple", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }

        [TestMethod]
        public void WithProfileAcceptHeader() {
            Object(Urls.MostSimple1, "MostSimple", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectProfile);
        }

        [TestMethod]
        public void AttemptWithInvalidProfileAcceptHeader() {
            Object(Urls.MostSimple1, null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.Homepage);
        }

        [TestMethod]
        public void Immutable() {
            Object(Urls.Immutable1, "Immutable");
        }

        [TestMethod]
        public void WithValue() {
            Object(Urls.WithValue1, "WithValue");
        }

        [TestMethod]
        public void WithReference() {
            Object(Urls.WithReference1, "WithReference");
        }

        [TestMethod]
        public void WithCollection() {
            Object(Urls.WithCollection1, "WithCollection");
        }

        [TestMethod]
        public void WithScalars() {
            Object(Urls.WithScalars1, "WithScalars");
        }

        [TestMethod]
        public void WithAction() {
            Object(Urls.WithActionObject1, "WithAction");
        }


        [TestMethod]
        public void WithAttachments() {
            Object(Urls.WithAttachments1, "WithAttachments");
        }



        [TestMethod]
        public void AttemptGetNonExistentInstance() {
            Object(Urls.NameSpace + @"MostSimple/999", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptGetNonExistentDomainType() {
            Object(Urls.NameSpace + @"MostSilly/1", null, null, Methods.Get, Codes.NotFound);
        }


        [TestMethod]
        public void AttemptGetWithQueryString() {
            Object(Urls.MostSimple1, null, "Id=1");
        }


        [TestMethod] 
        public void AttemptGetWithMalformedQueryString() {
            // don't want the parms urlencoded 
            string url = Urls.MostSimple1 + "?x-ro-validate-only=malformed";
            Object(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }

        //Eager rendering
        [TestMethod]
        public void VerySimpleEager()
        {
            Object(Urls.VerySimpleEager1, "VerySimpleEager");
        }
    }
}