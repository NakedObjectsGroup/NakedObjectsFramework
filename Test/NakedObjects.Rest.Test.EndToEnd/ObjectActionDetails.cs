// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass, Ignore]
    public class ObjectActionDetails : AbstractActionDetails {
        protected override string BaseUrl {
            get { return Urls.Objects + Urls.WithActionObject1 + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Object-Action-Details-"; }
        }

        [TestMethod]
        public void AnActionReturnsVoidWithParameters() {
            DoAnActionReturnsVoidWithParameters();
        }

        [TestMethod]
        public void ADisabledAction() {
            DoADisabledAction();
        }

        [TestMethod]
        public void ADisabledCollectionAction() {
            DoADisabledCollectionAction();
        }

        [TestMethod]
        public void ADisabledQueryAction() {
            DoADisabledQueryAction();
        }

        [TestMethod]
        public void AnAction() {
            DoAnAction();
        }

        [TestMethod]
        public void AnActionAnnotatedIdempotent() {
            DoAnActionAnnotatedIdempotent();
        }

        [TestMethod]
        public void AnActionAnnotatedIdempotentReturnsNull() {
            DoAnActionAnnotatedIdempotentReturnsNull();
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
        public void AnActionReturnsCollection() {
            DoAnActionReturnsCollection();
        }

        [TestMethod]
        public void AnActionReturnsCollectionEmpty() {
            DoAnActionReturnsCollectionEmpty();
        }

        [TestMethod]
        public void AnActionReturnsCollectionNull() {
            DoAnActionReturnsCollectionNull();
        }

        [TestMethod]
        public void AnActionReturnsCollectionWithParameters() {
            DoAnActionReturnsCollectionWithParameters();
        }

        [TestMethod]
        public void AnActionReturnsCollectionWithScalarParameters() {
            DoAnActionReturnsCollectionWithScalarParameters();
        }

        [TestMethod]
        public void AnActionReturnsNull() {
            DoAnActionReturnsNull();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParameters() {
            DoAnActionReturnsObjectWithParameters();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParametersAnnotatedIdempotent() {
            DoAnActionReturnsObjectWithParametersAnnotatedIdempotent();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
            DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly();
        }

        [TestMethod]
        public void AnActionReturnsQueryable() {
            DoAnActionReturnsQueryable();
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
        public void AnActionReturnsScalar() {
            DoAnActionReturnsScalar();
        }

        [TestMethod]
        public void AnActionReturnsScalarEmpty() {
            DoAnActionReturnsScalarEmpty();
        }

        [TestMethod]
        public void AnActionReturnsScalarNull() {
            DoAnActionReturnsScalarNull();
        }

        [TestMethod]
        public void AnActionReturnsScalarWithParameters() {
            DoAnActionReturnsScalarWithParameters();
        }

        [TestMethod]
        public void AnActionReturnsVoid() {
            DoAnActionReturnsVoid();
        }

        [TestMethod]
        public void AnActionWithDateTimeParm() {
            DoAnActionWithDateTimeParm();
        }

        [TestMethod]
        public void AnActionWithOptionalParm() {
            DoAnActionWithOptionalParm();
        }


        [TestMethod]
        public void AnActionWithReferenceParameter() {
            DoAnActionWithReferenceParameter();
        }

        [TestMethod]
        public void AnActionWithValueParameter() {
            DoAnActionWithValueParameter();
        }

        [TestMethod]
        public void AnActionWithValueParameterWithChoices() {
            DoAnActionWithValueParameterWithChoices();
        }

        //Tests specific to object-actions
        [TestMethod]
        public void AttemptToGetAPropertyAsAnAction() {
            TestActionDetails("Id", Methods.Get, Codes.NotFound);
        }

        [TestMethod]
        public void AttemptPutActionDetails() {
            DoAttemptPutActionDetails();
        }

        [TestMethod]
        public void AttemptPostActionDetails() {
            DoAttemptPostActionDetails();
        }

        [TestMethod]
        public void AttemptDeleteActionDetails() {
            DoAttemptDeleteActionDetails();
        }

        [TestMethod]
        public void WithGenericAcceptHeader() {
            DoWithGenericAcceptHeader();
        }

        [TestMethod]
        public void WithProfileAcceptHeader() {
            DoWithProfileAcceptHeader();
        }

        [TestMethod]
        public void AttemptWithInvalidProfileAcceptHeader() {
            DoAttemptWithInvalidProfileAcceptHeader();
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
    }
}