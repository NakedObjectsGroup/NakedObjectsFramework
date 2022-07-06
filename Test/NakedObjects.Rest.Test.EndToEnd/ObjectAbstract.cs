// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

/// <summary>
/// </summary>
public abstract class ObjectAbstract {
    private static readonly string objectsUrl = Urls.Objects;

    protected virtual string FilePrefix => "NO PREFIX SPECIFIED";

    protected void Object(string oid, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded, string acceptHeader = null) {
        if (fileName != null) {
            fileName = FilePrefix + fileName;
        }

        Helpers.Helpers.TestResponse(objectsUrl + oid, fileName, body, method, code, acceptHeader);
    }
}