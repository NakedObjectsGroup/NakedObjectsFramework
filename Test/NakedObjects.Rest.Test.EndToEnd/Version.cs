// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class Version : AbstractSecondaryResource {
        protected override string Filename() {
            return "Version";
        }

        protected override string ResourceUrl() {
            return Urls.Version;
        }

        protected override string ProfileType() {
            return MediaTypes.Version;
        }

        [TestMethod]
        public override void GetResource() {
            DoGetResource();
        }

        [TestMethod, Ignore]
        public override void WithGenericAcceptHeader() {
            DoWithGenericAcceptHeader();
        }

        [TestMethod, Ignore]
        public override void WithProfileAcceptHeader() {
            DoWithProfileAcceptHeader();
        }

        [TestMethod, Ignore]
        public override void AttemptWithInvalidProfileAcceptHeader() {
            DoAttemptWithInvalidProfileAcceptHeader();
        }

        [TestMethod, Ignore]
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

        [TestMethod, Ignore]
        public override void AttemptWithDomainModelMalformed() {
            DoAttemptWithDomainModelMalformed();
        }
    }
}