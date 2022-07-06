// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

[TestClass]
public class DomainTypeActionInvokeTests {
    private static readonly string withActionObjectTypeActions = $@"{Urls.DomainTypes}{Urls.NameSpace}WithActionObject{Urls.TypeActions}";
    private static readonly string withActionTypeActions = $@"{Urls.DomainTypes}{Urls.NameSpace}WithAction{Urls.TypeActions}";

    private static readonly string isSubtypeOf = $@"{withActionObjectTypeActions}isSubtypeOf/invoke?supertype={Urls.NameSpace}";
    private static readonly string isSupertypeOf = $@"{withActionTypeActions}isSupertypeOf/invoke?subtype={Urls.NameSpace}";

    [TestMethod]
    public void IsSubtypeOf() {
        Helpers.Helpers.TestResponse($"{isSubtypeOf}WithAction", "IsSubtypeOf");
    }

    [TestMethod]
    public void IsSubtypeOfFalse() {
        Helpers.Helpers.TestResponse($"{isSubtypeOf}MostSimple", "IsSubtypeOfFalse");
    }

    [TestMethod]
    public void IsSubtypeOfNonExistentType() {
        Helpers.Helpers.TestResponse($"{isSubtypeOf}NoSuchClass", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void IsSubtypeOfMalformed() {
        var url = $@"{withActionObjectTypeActions}isSubtypeOf/invoke?subtype={Urls.NameSpace}MostSimple";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    [TestMethod]
    public void IsSubtypeOfNoArgs() {
        var url = $@"{withActionObjectTypeActions}isSubtypeOf/invoke";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    [TestMethod]
    public void IsSupertypeOf() {
        Helpers.Helpers.TestResponse($@"{isSupertypeOf}WithActionObject", "IsSupertypeOf");
    }

    [TestMethod]
    public void IsSupertypeOfFalse() {
        Helpers.Helpers.TestResponse($@"{isSupertypeOf}MostSimple", "IsSupertypeOfFalse");
    }

    [TestMethod]
    public void IsSupertypeOfNonExistentType() {
        Helpers.Helpers.TestResponse($"{isSupertypeOf}NoSuchClass", null, null, Methods.Get, Codes.NotFound);
    }

    [TestMethod]
    public void IsSupertypeOfMalformed() {
        var url = $@"{withActionObjectTypeActions}isSupertypeOf/invoke?supertype={Urls.NameSpace}WithActionObject";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    [TestMethod]
    public void IsSupertypeOfNoArgs() {
        var url = $@"{withActionObjectTypeActions}isSupertypeOf/invoke";
        Helpers.Helpers.TestResponse(url, null, null, Methods.Get, Codes.SyntacticallyInvalid);
    }

    [TestMethod]
    public void AttemptPut() {
        Helpers.Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        Helpers.Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptPost() {
        Helpers.Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        Helpers.Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
    }

    [TestMethod]
    public void AttemptDelete() {
        Helpers.Helpers.TestResponse(isSubtypeOf, null, null, Methods.Put, Codes.MethodNotValid);
        Helpers.Helpers.TestResponse(isSupertypeOf, null, null, Methods.Put, Codes.MethodNotValid);
    }
}