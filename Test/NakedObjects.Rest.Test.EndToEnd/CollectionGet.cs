// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class CollectionGet : CollectionAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Collection-Get-"; }
        }

        #endregion

        [TestMethod, Ignore]
        public void ACollection() {
            Collection("ACollection", "ACollection");
        }

        [TestMethod, Ignore]
        public void ASet() {
            Collection("ASet", "ASet");
        }

        [TestMethod, Ignore]
        public void ADisabledCollection() {
            Collection("ADisabledCollection", "ADisabledCollection");
        }

        [TestMethod, Ignore]
        public void AnEmptyCollection() {
            Collection("AnEmptyCollection", "AnEmptyCollection");
        }

        [TestMethod, Ignore]
        public void AnEmptySet() {
            Collection("AnEmptySet", "AnEmptySet");
        }

        [TestMethod, Ignore]
        public void AttemptToGetHiddenCollection() {
            Collection("AHiddenCollection", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod, Ignore]
        public void AttemptToGetANonExistentCollection() {
            Collection("ANonExistentCollection", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod, Ignore] //Fails!  Retrieves the property!
        public void AttemptToGetAPropertyAsACollection() {
            Collection("Id", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod, Ignore]
        public void WithGenericAcceptHeader() {
            Collection("ACollection", "ACollection", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }

        [TestMethod, Ignore]
        public void WithProfileAcceptHeader() {
            Collection("ACollection", "ACollection", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectCollection);
        }

        [TestMethod, Ignore]
        public void AttemptWithInvalidProfileAcceptHeader() {
            Collection("ACollection", null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
        }

        [TestMethod, Ignore]
        public void WithFormalDomainModel() {
            Collection("ACollection" + JsonRep.FormalDomainModeAsQueryString, "WithFormalDomainModel");
        }

        [TestMethod, Ignore]
        public void WithSimpleDomainModel() {
            Collection("ACollection" + JsonRep.SimpleDomainModeAsQueryString, "WithSimpleDomainModel");
        }

        [TestMethod, Ignore] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            Collection("ACollection" + JsonRep.DomainModeQueryStringMalformed, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }
    }
}