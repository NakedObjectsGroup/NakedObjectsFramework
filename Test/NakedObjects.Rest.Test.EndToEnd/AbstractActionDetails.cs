// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Newtonsoft.Json.Linq;

namespace RestfulObjects.Test.EndToEnd {
    public abstract class AbstractActionDetails : AbstractAction {
        protected void TestActionDetails(string actionName, string method = Methods.Get, Codes expectedErrorCode = Codes.Succeeded) {
            Helpers.TestResponse(BaseUrl + actionName, FilePrefix + actionName, null, method, expectedErrorCode);
        }

        public void DoAnActionReturnsVoidWithParameters() {
            TestActionDetails(Domain.AnActionReturnsVoidWithParameters);
        }

        public void DoADisabledAction() {
            TestActionDetails("ADisabledAction");
        }

        public void DoADisabledCollectionAction() {
            TestActionDetails("ADisabledCollectionAction");
        }

        public void DoADisabledQueryAction() {
            TestActionDetails("ADisabledQueryAction");
        }

        public void DoAnAction() {
            TestActionDetails("AnAction");
        }


        public void DoAnActionAnnotatedIdempotent() {
            TestActionDetails("AnActionAnnotatedIdempotent");
        }

        public void DoAnActionAnnotatedIdempotentReturnsNull() {
            TestActionDetails("AnActionAnnotatedIdempotentReturnsNull");
        }

        public void DoAnActionAnnotatedQueryOnly() {
            TestActionDetails("AnActionAnnotatedQueryOnly");
        }

        public void DoAnActionAnnotatedQueryOnlyReturnsNull() {
            TestActionDetails("AnActionAnnotatedQueryOnlyReturnsNull");
        }

        public void DoAnActionReturnsCollection() {
            TestActionDetails("AnActionReturnsCollection");
        }

        public void DoAnActionReturnsCollectionEmpty() {
            TestActionDetails("AnActionReturnsCollectionEmpty");
        }


        public void DoAnActionReturnsCollectionNull() {
            TestActionDetails("AnActionReturnsCollectionNull");
        }

        public void DoAnActionReturnsCollectionWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionDetails("AnActionReturnsCollectionWithParameters");
        }

        public void DoAnActionReturnsCollectionWithScalarParameters() {
            JObject parms = Parm1Is100Parm2IsFred();
            TestActionDetails("AnActionReturnsCollectionWithScalarParameters");
        }

        public void DoAnActionReturnsNull() {
            TestActionDetails("AnActionReturnsNull");
        }

        public void DoAnActionReturnsObjectWithParameters() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionDetails("AnActionReturnsObjectWithParameters");
        }

        public void DoAnActionReturnsObjectWithParametersAnnotatedIdempotent() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionDetails("AnActionReturnsObjectWithParametersAnnotatedIdempotent");
        }

        public void DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
            JObject parms = Parm1Is101Parm2IsMostSimple1();
            TestActionDetails("AnActionReturnsObjectWithParametersAnnotatedQueryOnly");
        }

        public void DoAnActionReturnsQueryable() {
            TestActionDetails("AnActionReturnsQueryable");
        }

        public void DoAnActionReturnsQueryableWithParameters() {
            TestActionDetails("AnActionReturnsQueryableWithParameters");
        }

        public void DoAnActionReturnsQueryableWithScalarParameters() {
            TestActionDetails(Domain.AnActionReturnsQueryableWithScalarParameters);
        }

        public void DoAnActionReturnsScalar() {
            TestActionDetails("AnActionReturnsScalar");
        }

        public void DoAnActionReturnsScalarEmpty() {
            TestActionDetails("AnActionReturnsScalarEmpty");
        }

        public void DoAnActionReturnsScalarNull() {
            TestActionDetails("AnActionReturnsScalarNull");
        }

        public void DoAnActionReturnsScalarWithParameters() {
            TestActionDetails("AnActionReturnsScalarWithParameters");
        }

        public void DoAnActionReturnsVoid() {
            TestActionDetails("AnActionReturnsVoid");
        }

        public void DoAnActionWithDateTimeParm() {
            TestActionDetails("AnActionWithDateTimeParm");
        }

        public void DoAnActionWithOptionalParm() {
            TestActionDetails("AnActionWithOptionalParm");
        }


        public void DoAnActionWithReferenceParameter() {
            TestActionDetails("AnActionWithReferenceParameter");
        }

        public void DoAnActionWithValueParameter() {
            TestActionDetails("AnActionWithValueParameter");
        }

        public void DoAnActionWithValueParameterWithChoices() {
            TestActionDetails("AnActionWithValueParameterWithChoices");
        }

        public void DoAttemptPutActionDetails() {
            TestActionDetails("AnAction", Methods.Put, Codes.MethodNotValid);
        }

        public void DoAttemptPostActionDetails() {
            TestActionDetails("AnAction", Methods.Post, Codes.MethodNotValid);
        }

        public void DoAttemptDeleteActionDetails() {
            TestActionDetails("AnAction", Methods.Put, Codes.MethodNotValid);
        }


        protected void DoWithGenericAcceptHeader() {
            Helpers.TestResponse(BaseUrl + "AnAction", FilePrefix + "AnAction", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
        }


        protected void DoWithProfileAcceptHeader() {
            Helpers.TestResponse(BaseUrl + "AnAction", FilePrefix + "AnAction", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectAction);
        }

        protected void DoAttemptWithInvalidProfileAcceptHeader() {
            Helpers.TestResponse(BaseUrl + "AnAction", null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
        }

        protected void DoWithFormalDomainModel() {
            string url = BaseUrl + "AnAction" + JsonRep.FormalDomainModeAsQueryString;
            Helpers.TestResponse(url, FilePrefix + "WithFormalDomainModel");
        }

        protected void DoWithSimpleDomainModel() {
            string url = BaseUrl + "AnAction" + JsonRep.SimpleDomainModeAsQueryString;
            Helpers.TestResponse(url, FilePrefix + "WithSimpleDomainModel");
        }

        protected void DoAttemptWithMalformedDomainModel() {
            string url = BaseUrl + "AnAction" + JsonRep.DomainModeQueryStringMalformed;
            Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
        }
    }
}