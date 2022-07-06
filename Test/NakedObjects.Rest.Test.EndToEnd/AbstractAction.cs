﻿// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;
using Newtonsoft.Json.Linq;

namespace NakedObjects.Rest.Test.EndToEnd;

public class AbstractAction {
    protected virtual string BaseUrl => "";

    protected virtual string FilePrefix => "";

    #region Helpers

    protected void TestActionInvoke(string actionName, string body = null, string method = Methods.Get, Codes expectedErrorCode = Codes.Succeeded, string acceptHeader = null) {
        Helpers.Helpers.TestResponse($@"{BaseUrl}{actionName}/invoke", FilePrefix + actionName, body, method, expectedErrorCode, acceptHeader);
    }

    protected static JObject Parm1Is101Parm2IsMostSimple1() {
        var parm1 = new JObject(new JProperty("value", 101));
        var parm2 = new JObject(new JProperty("value", JsonRep.MostSimple1AsRef()));
        return new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2));
    }

    protected static JObject Parm1Is101() {
        var parm1 = new JObject(new JProperty("value", 101));

        return new JObject(new JProperty("parm1", parm1));
    }

    protected static JObject Parm1Is100Parm2IsFred() {
        var parm1 = new JObject(new JProperty("value", 100));
        var parm2 = new JObject(new JProperty("value", "fred"));
        return new JObject(new JProperty("parm1", parm1), new JProperty("parm2", parm2));
    }

    protected static string Parm1Is100Parm2IsFredAsQueryString() => "?parm1=100&parm2=fred";

    #endregion
}