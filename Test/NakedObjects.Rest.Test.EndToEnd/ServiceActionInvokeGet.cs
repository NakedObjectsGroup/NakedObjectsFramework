// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ServiceActionInvokeGet : AbstractActionInvokeGet {
        protected override string BaseUrl {
            get { return Urls.Services + Urls.WithActionService + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Service-Action-Invoke-Get-"; }
        }

        [TestMethod]
        public void ADisabledQueryAction() {
            DoADisabledQueryAction();
        }

        [TestMethod, Ignore]
        public void InvokeGetActionWithPut() {
            DoInvokeGetActionWithPut();
        }

        [TestMethod, Ignore]
        public void AnActionAnnotatedQueryOnly() {
            DoAnActionAnnotatedQueryOnly();
        }

        [TestMethod, Ignore]
        public void AnActionAnnotatedQueryOnlyReturnsNull() {
            DoAnActionAnnotatedQueryOnlyReturnsNull();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
            DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsObjectWithParameterAnnotatedQueryOnly() {
            DoAnActionReturnsObjectWithParameterAnnotatedQueryOnly();
        }

        [TestMethod, Ignore] //https://restfulobjects.codeplex.com/workitem/25
        public void SyntacticallyMalformedParamsAsEncodedMap1() {
            DoSyntacticallyMalformedParamsAsEncodedMap1();
        }

        [TestMethod, Ignore]
        public void SyntacticallyMalformedParamsAsEncodedMap2() {
            DoSyntacticallyMalformedParamsAsEncodedMap2();
        }

        [TestMethod]
        public void AnActionReturnsQueryable() {
            DoAnActionReturnsQueryable();
        }

        [TestMethod, Ignore]
        public void WithFormalDomainModel() {
            DoWithFormalDomainModel();
        }

        [TestMethod, Ignore]
        public void WithSimpleDomainModel() {
            DoWithSimpleDomainModel();
        }

        [TestMethod, Ignore] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            DoAttemptWithMalformedDomainModel();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsQueryableWithParameters() {
            DoAnActionReturnsQueryableWithParameters();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsQueryableWithScalarParameters() {
            DoAnActionReturnsQueryableWithScalarParameters();
        }

        [TestMethod, Ignore]
        public void ScalarParametersAsQueryString() {
            DoScalarParametersAsQueryString();
        }

        [TestMethod, Ignore]
        public void SyntacticallyMalformedQueryString() {
            DoSyntacticallyMalformedQueryString();
        }

        [TestMethod, Ignore]
        public void SemanticallyMalformedQueryString() {
            DoSemanticallyMalformedQueryString();
        }

        [TestMethod, Ignore]
        public void AnErrorQuery() {
            DoAnErrorQuery();
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
    }
}