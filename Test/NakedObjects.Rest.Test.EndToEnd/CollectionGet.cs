// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class CollectionGet : CollectionAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Collection-Get-"; }
        }

        #endregion

        [TestMethod]
        public void ACollection() {
            Collection("ACollection", "ACollection");
        }

        [TestMethod]
        public void ASet() {
            Collection("ASet", "ASet");
        }

        [TestMethod]
        public void ADisabledCollection() {
            Collection("ADisabledCollection", "ADisabledCollection");
        }

        [TestMethod]
        public void AnEmptyCollection() {
            Collection("AnEmptyCollection", "AnEmptyCollection");
        }

        [TestMethod]
        public void AnEmptySet() {
            Collection("AnEmptySet", "AnEmptySet");
        }

        [TestMethod]
        public void AttemptToGetHiddenCollection() {
            Collection("AHiddenCollection", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptToGetANonExistentCollection() {
            Collection("ANonExistentCollection", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod] //Fails!  Retrieves the property!
        public void AttemptToGetAPropertyAsACollection() {
            Collection("Id", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void WithGenericAcceptHeader() {
            Collection("ACollection", "ACollection", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }

        [TestMethod]
        public void WithProfileAcceptHeader() {
            Collection("ACollection", "ACollection", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectCollection);
        }

        [TestMethod]
        public void AttemptWithInvalidProfileAcceptHeader() {
            Collection("ACollection", null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
        }

        [TestMethod]
        public void WithFormalDomainModel() {
            Collection("ACollection" + JsonRep.FormalDomainModeAsQueryString, "WithFormalDomainModel");
        }

        [TestMethod]
        public void WithSimpleDomainModel() {
            Collection("ACollection" + JsonRep.SimpleDomainModeAsQueryString, "WithSimpleDomainModel");
        }

        [TestMethod] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            Collection("ACollection" + JsonRep.DomainModeQueryStringMalformed, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }
    }
}