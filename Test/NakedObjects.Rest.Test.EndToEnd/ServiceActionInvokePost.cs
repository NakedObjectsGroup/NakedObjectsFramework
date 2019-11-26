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

        [TestMethod, Ignore]
        public void AnActionReturnsVoidWithParameters() {
            DoAnActionReturnsVoidWithParameters();
        }

        [TestMethod, Ignore]
        public void SyntacticallyMalformedParameters() {
            DoSyntacticallyMalformedParameters();
        }

        [TestMethod, Ignore]
        public void SemanticallyMalformedParameters() {
            DoSemanticallyMalformedParameters();
        }

        [TestMethod, Ignore]
        public void ADisabledAction() {
            DoADisabledAction();
        }

        [TestMethod, Ignore]
        public void ADisabledCollectionAction() {
            DoADisabledCollectionAction();
        }

        [TestMethod, Ignore]
        public void AnAction() {
            DoAnAction();
        }

        [TestMethod, Ignore]
        public void AttemptInvokePostActionWithGet() {
            DoAttemptInvokePostActionWithGet();
        }

        [TestMethod, Ignore]
        public void AttemptInvokePostActionWithPut() {
            DoAttemptInvokePostActionWithPut();
        }

        [TestMethod, Ignore]
        public void AttemptInvokePutActionWithPost() {
            DoAttemptInvokePutActionWithPost();
        }

        [TestMethod, Ignore]
        public void AttemptInvokeGetActionWithPost() {
            DoAttemptInvokeGetActionWithPost();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsCollection() {
            DoAnActionReturnsCollection();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsCollectionEmpty() {
            DoAnActionReturnsCollectionEmpty();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsCollectionNull() {
            DoAnActionReturnsCollectionNull();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsCollectionWithParameters() {
            DoAnActionReturnsCollectionWithParameters();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsCollectionWithScalarParameters() {
            DoAnActionReturnsCollectionWithScalarParameters();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsNull() {
            DoAnActionReturnsNull();
        }

        [TestMethod]
        public void AnActionReturnsObjectWithParameters() {
            DoAnActionReturnsObjectWithParameters();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsScalar() {
            DoAnActionReturnsScalar();
        }

        [TestMethod, Ignore]  // Must FIX !!
        public void AnActionReturnsScalarEmpty() {
            DoAnActionReturnsScalarEmpty();
        }

        [TestMethod, Ignore]
        public void AnActionReturnsScalarNull() {
            DoAnActionReturnsScalarNull();
        }

        [TestMethod, Ignore]
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