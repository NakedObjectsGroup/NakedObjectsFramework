// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;

public abstract class AbstractActionInvokeGet : AbstractAction {
    protected void DoADisabledQueryAction() {
        TestActionInvoke("ADisabledQueryAction", null, Methods.Get, Codes.Forbidden);
    }

    protected void DoAnActionAnnotatedQueryOnly() {
        TestActionInvoke("AnActionAnnotatedQueryOnly");
    }

    protected void DoAnActionAnnotatedQueryOnlyReturnsNull() {
        TestActionInvoke("AnActionAnnotatedQueryOnlyReturnsNull", JsonRep.Empty());
    }

    protected void DoAnActionReturnsObjectWithParametersAnnotatedQueryOnly() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionInvoke("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", parms.ToString());
    }

    protected void DoAnActionReturnsObjectWithParameterAnnotatedQueryOnly() {
        var parms = Parm1Is101();
        TestActionInvoke("AnActionReturnsObjectWithParameterAnnotatedQueryOnly", parms.ToString());
    }

    //Contrast with SyntacticallyMalformedQueryString
    protected void DoSyntacticallyMalformedParamsAsEncodedMap1() {
        var parm1 = new JObject(new JProperty("vaalue", 101)); //mis-spelled
        var parm2 = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
        var parms = new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2));
        TestActionInvoke("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", parms.ToString(), Methods.Get, Codes.SyntacticallyInvalid);
    }

    protected void DoSyntacticallyMalformedParamsAsEncodedMap2() {
        var parm1 = new JObject(new JProperty("value", 101));
        var parm2 = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
        var parms = new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2), new JProperty("parm3", parm2)); //Additional param
        TestActionInvoke("AnActionReturnsObjectWithParametersAnnotatedQueryOnly", parms.ToString(), Methods.Get, Codes.SyntacticallyInvalid);
    }

    protected void DoAnActionReturnsQueryable() {
        TestActionInvoke("AnActionReturnsQueryable");
    }

    protected void DoWithFormalDomainModel() {
        var url = $"{BaseUrl}AnActionReturnsQueryable/invoke{JsonRep.FormalDomainModeAsQueryString}";
        Helpers.Helpers.TestResponse(url, $"{FilePrefix}WithFormalDomainModel-Invoke");
    }

    protected void DoWithSimpleDomainModel() {
        var url = $"{BaseUrl}AnActionReturnsQueryable/invoke{JsonRep.SimpleDomainModeAsQueryString}";
        Helpers.Helpers.TestResponse(url, $"{FilePrefix}WithSimpleDomainModel-Invoke");
    }

    protected void DoAttemptWithMalformedDomainModel() {
        var url = $"{BaseUrl}AnActionReturnsQueryable/invoke{JsonRep.DomainModeQueryStringMalformed}";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    protected void DoAnActionReturnsQueryableWithParameters() {
        var parms = Parm1Is101Parm2IsMostSimple1();
        TestActionInvoke("AnActionReturnsQueryableWithParameters", parms.ToString());
    }

    protected void DoAnActionReturnsQueryableWithScalarParameters() {
        var parms = Parm1Is100Parm2IsFred();
        TestActionInvoke(Domain.AnActionReturnsQueryableWithScalarParameters, parms.ToString());
    }

    //Same as 'AnActionReturnsQueryableWithScalarParameters' but with params as query string in the URL rather than a body.
    protected void DoScalarParametersAsQueryString() {
        const string actionName = Domain.AnActionReturnsQueryableWithScalarParameters;
        var url = $@"{BaseUrl}{actionName}/invoke{Parm1Is100Parm2IsFredAsQueryString()}";
        Helpers.Helpers.TestResponse(url, FilePrefix + actionName);
    }

    protected void DoSyntacticallyMalformedQueryString() {
        const string actionName = Domain.AnActionReturnsQueryableWithScalarParameters;
        var url = $@"{BaseUrl}{actionName}/invoke?parm1=100&param2=fred"; //mis-named parameter
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);

        url = $@"{BaseUrl}{actionName}/invoke?parm1=100&parm2=fred&parm3=1"; //extra-parameter
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    protected void DoSemanticallyMalformedQueryString() {
        const string actionName = Domain.AnActionReturnsQueryableWithScalarParameters;
        var url = $@"{BaseUrl}{actionName}/invoke?parm1=foo&parm2=fred"; //i.e. a string that should be a number
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    protected void DoActionWithScalarParametersAsQueryString() {
        const string actionName = Domain.AnActionReturnsQueryableWithScalarParameters;
        var url = $@"{BaseUrl}{actionName}/invoke{Parm1Is100Parm2IsFredAsQueryString()}";
        Helpers.Helpers.TestResponse(url, FilePrefix + actionName);
    }

    protected void DoAnErrorQuery() {
        TestActionInvoke("AnErrorQuery", null, Methods.Get, Codes.ServerException);
    }

    protected void DoInvokeGetActionWithPut() {
        // currently allowed - may need revisiting
        TestActionInvoke("AnActionAnnotatedQueryOnly", JsonRep.Empty(), Methods.Put);
    }

    public abstract void WithGenericAcceptHeader();

    protected void DoWithGenericAcceptHeader() {
        TestActionInvoke("AnActionReturnsQueryable", null, Methods.Get, Codes.Succeeded, MediaTypes.Json);
    }

    public abstract void WithProfileAcceptHeader();

    protected void DoWithProfileAcceptHeader() {
        TestActionInvoke("AnActionReturnsQueryable", null, Methods.Get, Codes.Succeeded, MediaTypes.ActionResult);
    }

    public abstract void AttemptWithInvalidProfileAcceptHeader();

    protected void DoAttemptWithInvalidProfileAcceptHeader() {
        TestActionInvoke("AnActionReturnsQueryable", null, Methods.Get, Codes.WrongMediaType, MediaTypes.ObjectProfile);
    }
}