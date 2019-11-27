// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestfulObjects.Test.EndToEnd {
    [TestClass]
    public class ServiceActionInvokePost : AbstractActionInvokePost {
        protected override string BaseUrl {
            get { return Urls.Services + Urls.WithActionService + Urls.Actions; }
        }

        protected override string FilePrefix {
            get { return "Service-Action-Invoke-Post-"; }
        }

        [TestMethod]
        public void AnActionReturnsVoidWithParameters() {
            DoAnActionReturnsVoidWithParameters();
        }

        [TestMethod]
        public void SyntacticallyMalformedParameters() {
            DoSyntacticallyMalformedParameters();
        }

        [TestMethod]
        public void SemanticallyMalformedParameters() {
            DoSemanticallyMalformedParameters();
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
        public void AnAction() {
            DoAnAction();
        }

        [TestMethod]
        public void AttemptInvokePostActionWithGet() {
            DoAttemptInvokePostActionWithGet();
        }

        [TestMethod]
        public void AttemptInvokePostActionWithPut() {
            DoAttemptInvokePostActionWithPut();
        }

        [TestMethod]
        public void AttemptInvokePutActionWithPost() {
            DoAttemptInvokePutActionWithPost();
        }

        [TestMethod]
        public void AttemptInvokeGetActionWithPost() {
            DoAttemptInvokeGetActionWithPost();
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

        [TestMethod, Ignore]
        public void AnActionReturnsVoid() {
            DoAnActionReturnsVoid();
        }

        [TestMethod, Ignore]
        public void AnActionWithDateTimeParm() {
            DoAnActionWithDateTimeParm();
        }

        [TestMethod, Ignore]
        public void AnActionWithOptionalParm() {
            DoAnActionWithOptionalParm();
        }


        [TestMethod, Ignore]
        public void AnActionWithReferenceParameter() {
            DoAnActionWithReferenceParameter();
        }

        [TestMethod, Ignore]
        public void AnActionWithValueParameter() {
            DoAnActionWithValueParameter();
        }

        [TestMethod, Ignore]
        public void AnActionWithValueParameterWithChoices() {
            DoAnActionWithValueParameterWithChoices();
        }

        [TestMethod, Ignore]
        public void AnError() {
            DoAnError();
        }

        [TestMethod, Ignore]
        public void AnErrorCollection() {
            DoAnErrorCollection();
        }

        [TestMethod, Ignore]
        public void ParameterValidateOnlyGood() {
            DoParameterValidateOnlyGood();
        }

        [TestMethod, Ignore]
        public void ParameterValidateOnlyBad() {
            DoParameterValidateOnlyBad();
        }

        [TestMethod, Ignore]
        public void AttemptInvalidParameters() {
            DoAttemptInvalidParameters();
        }

        [TestMethod, Ignore]
        public void AttemptInvalidJson() {
            DoAttemptInvalidJson();
        }

        [TestMethod, Ignore]
        public void ValidateOnlyParameters() {
            DoValidateOnlyParameters();
        }
    }
}