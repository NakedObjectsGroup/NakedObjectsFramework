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

        [TestMethod]
        public void InvokeGetActionWithPut() {
            DoInvokeGetActionWithPut();
        }

        [TestMethod]
        public void AnActionAnnotatedQueryOnly() {
            DoAnActionAnnotatedQueryOnly();
        }

        [TestMethod]
        public void AnActionAnnotatedQueryOnlyReturnsNull() {
            DoAnActionAnnotatedQueryOnlyReturnsNull();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
            DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParameterAnnotatedQueryOnly() {
            DoAnActionReturnsObjectWithParameterAnnotatedQueryOnly();
        }

        [TestMethod] //https://restfulobjects.codeplex.com/workitem/25
        public void SyntacticallyMalformedParamsAsEncodedMap1() {
            DoSyntacticallyMalformedParamsAsEncodedMap1();
        }

        [TestMethod]
        public void SyntacticallyMalformedParamsAsEncodedMap2() {
            DoSyntacticallyMalformedParamsAsEncodedMap2();
        }

        [TestMethod]
        public void AnActionReturnsQueryable() {
            DoAnActionReturnsQueryable();
        }

        [TestMethod]
        public void WithFormalDomainModel() {
            DoWithFormalDomainModel();
        }

        [TestMethod]
        public void WithSimpleDomainModel() {
            DoWithSimpleDomainModel();
        }

        [TestMethod] //http://restfulobjects.codeplex.com/workitem/26
        public void AttemptWithMalformedDomainModel() {
            DoAttemptWithMalformedDomainModel();
        }

        [TestMethod]
        public void AnActionReturnsQueryableWithParameters() {
            DoAnActionReturnsQueryableWithParameters();
        }

        [TestMethod]
        public void AnActionReturnsQueryableWithScalarParameters() {
            DoAnActionReturnsQueryableWithScalarParameters();
        }

        [TestMethod]
        public void ScalarParametersAsQueryString() {
            DoScalarParametersAsQueryString();
        }

        [TestMethod]
        public void SyntacticallyMalformedQueryString() {
            DoSyntacticallyMalformedQueryString();
        }

        [TestMethod]
        public void SemanticallyMalformedQueryString() {
            DoSemanticallyMalformedQueryString();
        }

        [TestMethod]
        public void AnErrorQuery() {
            DoAnErrorQuery();
        }

        [TestMethod]
        public override void WithGenericAcceptHeader() {
            DoWithGenericAcceptHeader();
        }

        [TestMethod]
        public override void WithProfileAcceptHeader() {
            DoWithProfileAcceptHeader();
        }

        [TestMethod]
        public override void AttemptWithInvalidProfileAcceptHeader() {
            DoAttemptWithInvalidProfileAcceptHeader();
        }
    }
}