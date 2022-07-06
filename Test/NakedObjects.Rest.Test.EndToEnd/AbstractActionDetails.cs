// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

public abstract class AbstractActionDetails : AbstractAction {
    protected void TestActionDetails(string actionName, string method = Methods.Get, Codes expectedErrorCode = Codes.Succeeded) {
        Helpers.Helpers.TestResponse(BaseUrl + actionName, FilePrefix + actionName, null, method, expectedErrorCode);
    }

    protected void DoAnActionReturnsVoidWithParameters() {
        TestActionDetails(Domain.AnActionReturnsVoidWithParameters);
    }

    protected void DoADisabledAction() {
        TestActionDetails("ADisabledAction");
    }

    protected void DoADisabledCollectionAction() {
        TestActionDetails("ADisabledCollectionAction");
    }

    protected void DoADisabledQueryAction() {
        TestActionDetails("ADisabledQueryAction");
    }

    protected void DoAnAction() {
        TestActionDetails("AnAction");
    }

    protected void DoAnActionAnnotatedIdempotent() {
        TestActionDetails("AnActionAnnotatedIdempotent");
    }

    protected void DoAnActionAnnotatedIdempotentReturnsNull() {
        TestActionDetails("AnActionAnnotatedIdempotentReturnsNull");
    }

    protected void DoAnActionAnnotatedQueryOnly() {
        TestActionDetails("AnActionAnnotatedQueryOnly");
    }

    protected void DoAnActionAnnotatedQueryOnlyReturnsNull() {
        TestActionDetails("AnActionAnnotatedQueryOnlyReturnsNull");
    }

    protected void DoAnActionReturnsCollection() {
        TestActionDetails("AnActionReturnsCollection");
    }

    protected void DoAnActionReturnsCollectionEmpty() {
        TestActionDetails("AnActionReturnsCollectionEmpty");
    }

    protected void DoAnActionReturnsCollectionNull() {
        TestActionDetails("AnActionReturnsCollectionNull");
    }

    protected void DoAnActionReturnsCollectionWithParameters() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionDetails("AnActionReturnsCollectionWithParameters");
    }

    protected void DoAnActionReturnsCollectionWithScalarParameters() {
        var parms = Parm1Is100Parm2IsFred();
        TestActionDetails("AnActionReturnsCollectionWithScalarParameters");
    }

    protected void DoAnActionReturnsNull() {
        TestActionDetails("AnActionReturnsNull");
    }

    protected void DoAnActionReturnsObjectWithParameters() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionDetails("AnActionReturnsObjectWithParameters");
    }

    protected void DoAnActionReturnsObjectWithParametersAnnotatedIdempotent() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionDetails("AnActionReturnsObjectWithParametersAnnotatedIdempotent");
    }

    protected void DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionDetails("AnActionReturnsObjectWithParametersAnnotatedQueryOnly");
    }

    protected void DoAnActionReturnsQueryable() {
        TestActionDetails("AnActionReturnsQueryable");
    }

    protected void DoAnActionReturnsQueryableWithParameters() {
        TestActionDetails("AnActionReturnsQueryableWithParameters");
    }

    protected void DoAnActionReturnsQueryableWithScalarParameters() {
        TestActionDetails(Domain.AnActionReturnsQueryableWithScalarParameters);
    }

    protected void DoAnActionReturnsScalar() {
        TestActionDetails("AnActionReturnsScalar");
    }

    protected void DoAnActionReturnsScalarEmpty() {
        TestActionDetails("AnActionReturnsScalarEmpty");
    }

    protected void DoAnActionReturnsScalarNull() {
        TestActionDetails("AnActionReturnsScalarNull");
    }

    protected void DoAnActionReturnsScalarWithParameters() {
        TestActionDetails("AnActionReturnsScalarWithParameters");
    }

    protected void DoAnActionReturnsVoid() {
        TestActionDetails("AnActionReturnsVoid");
    }

    protected void DoAnActionWithDateTimeParm() {
        TestActionDetails("AnActionWithDateTimeParm");
    }

    protected void DoAnActionWithOptionalParm() {
        TestActionDetails("AnActionWithOptionalParm");
    }

    protected void DoAnActionWithReferenceParameter() {
        TestActionDetails("AnActionWithReferenceParameter");
    }

    protected void DoAnActionWithValueParameter() {
        TestActionDetails("AnActionWithValueParameter");
    }

    protected void DoAnActionWithValueParameterWithChoices() {
        TestActionDetails("AnActionWithValueParameterWithChoices");
    }

    protected void DoAttemptPutActionDetails() {
        TestActionDetails("AnAction", Methods.Put, Codes.MethodNotValid);
    }

    protected void DoAttemptPostActionDetails() {
        TestActionDetails("AnAction", Methods.Post, Codes.MethodNotValid);
    }

    protected void DoAttemptDeleteActionDetails() {
        TestActionDetails("AnAction", Methods.Put, Codes.MethodNotValid);
    }

    protected void DoWithGenericAcceptHeader() {
        Helpers.Helpers.TestResponse($"{BaseUrl}AnAction", $"{FilePrefix}AnAction", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
    }

    protected void DoWithProfileAcceptHeader() {
        Helpers.Helpers.TestResponse($"{BaseUrl}AnAction", $"{FilePrefix}AnAction", null, Methods.Get, Codes.Succeeded, MediaTypes.ObjectAction);
    }

    protected void DoAttemptWithInvalidProfileAcceptHeader() {
        Helpers.Helpers.TestResponse($"{BaseUrl}AnAction", null, null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
    }

    protected void DoWithFormalDomainModel() {
        var url = $"{BaseUrl}AnAction{JsonRep.FormalDomainModeAsQueryString}";
        Helpers.Helpers.TestResponse(url, $"{FilePrefix}WithFormalDomainModel");
    }

    protected void DoWithSimpleDomainModel() {
        var url = $"{BaseUrl}AnAction{JsonRep.SimpleDomainModeAsQueryString}";
        Helpers.Helpers.TestResponse(url, $"{FilePrefix}WithSimpleDomainModel");
    }

    protected void DoAttemptWithMalformedDomainModel() {
        var url = $"{BaseUrl}AnAction{JsonRep.DomainModeQueryStringMalformed}";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }
}