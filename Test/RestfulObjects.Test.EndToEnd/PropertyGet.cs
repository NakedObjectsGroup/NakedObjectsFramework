// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class PropertyGet : PropertyAbstract {
        #region Helpers

        protected override string FilePrefix {
            get { return "Property-Get-"; }
        }

        #endregion

        [TestMethod]
        public void Int() {
            WithValue("AValue", "AValue");
        }

        [TestMethod]
        public void String() {
            WithValue("AStringValue", "AStringValue");
        }

        [TestMethod]
        public void DateTime() {
            WithValue("ADateTimeValue", "ADateTimeValue");
        }

        [TestMethod]
        public void AttemptGetNonExistentProperty() {
            WithValue("NoSuchProperty", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptGetACollectionAsProperty() {
            WithCollection("ACollection", null, null, Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void AReference() {
            WithReference("AReference", "AReference");
        }


        [TestMethod]
        public void WithGenericAcceptHeader() {
            WithValue("AValue", "AValue", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }

        [TestMethod]
        public void WithProfileAcceptHeader() {
            WithValue("AValue", "AValue", null, Methods.Get, Codes.Succeeded, MediaTypes.Property);
        }

        [TestMethod]
        public void AttemptWithInvalidProfileAcceptHeader() {
            WithValue("AValue", null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
        }


        [TestMethod]
        public void WithFormalDomainModel() {
            string url = withValuePropertiesGet + "AValue" + JsonRep.FormalDomainModeAsQueryString;
            Helpers.TestResponse(url, FilePrefix + "WithFormalDomainModel");
        }

        [TestMethod]
        public void WithSimpleDomainModel() {
            string url = withValuePropertiesGet + "AValue" + JsonRep.SimpleDomainModeAsQueryString;
            Helpers.TestResponse(url, FilePrefix + "WithSimpleDomainModel");
        }

        [TestMethod] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            string url = withValuePropertiesGet + "AValue" + JsonRep.DomainModeQueryStringMalformed;
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }
    }
}