// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class Home : AbstractSecondaryResource {
        protected override string Filename() {
            return "Home";
        }

        protected override string ResourceUrl() {
            return Urls.BaseUrl;
        }

        protected override string ProfileType() {
            return MediaTypes.Homepage;
        }

        [TestMethod]
        public override void GetResource() {
            DoGetResource();
        }

        [TestMethod]
        public override void WithGenericAcceptHeader() {
            DoWithGenericAcceptHeader();
        }

        [TestMethod]
        public override void WithProfileAcceptHeader() {
            DoWithProfileAcceptHeader();
        }

        [TestMethod, Ignore]
        public override void AttemptWithInvalidProfileAcceptHeader() {
            DoAttemptWithInvalidProfileAcceptHeader();
        }

        [TestMethod]
        public override void AttemptPost() {
            DoAttemptPost();
        }

        [TestMethod, Ignore]
        public override void AttemptPut() {
            DoAttemptPut();
        }

        [TestMethod, Ignore]
        public override void AttemptDelete() {
            DoAttemptDelete();
        }

        [TestMethod, Ignore]
        public override void WithFormalDomainModel() {
            DoWithFormalDomainModel();
        }

        [TestMethod, Ignore]
        public override void WithSimpleDomainModel() {
            DoWithSimpleDomainModel();
        }

        [TestMethod, Ignore] //http://restfulobjects.codeplex.com/workitem/26
        public override void AttemptWithDomainModelMalformed() {
            DoAttemptWithDomainModelMalformed();
        }
    }
}