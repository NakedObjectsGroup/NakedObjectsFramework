// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Rest.Test.EndToEnd.Helpers;

namespace NakedObjects.Rest.Test.EndToEnd;

public abstract class PropertyAbstract {
    protected static readonly string withValuePropertiesGet = Urls.Objects + Urls.WithValue1 + Urls.Properties;

    private static readonly string withValuePropertiesSet = Urls.Objects + Urls.WithValue2 + Urls.Properties;

    private static readonly string withReferenceProperties = Urls.Objects + Urls.WithReference1 + Urls.Properties;

    private static readonly string withCollectionProperties = Urls.Objects + Urls.WithCollection1 + Urls.Properties;

    protected virtual string FilePrefix => "NO PREFIX SPECIFIED";

    protected void WithValue(string propertyName, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded, string acceptHeaders = null) {
        var withValueProperties = method == Methods.Get ? withValuePropertiesGet : withValuePropertiesSet;

        Helpers.Helpers.TestResponse(withValueProperties + propertyName, FilePrefix + fileName, body, method, code, acceptHeaders);
    }

    protected void WithReference(string propertyName, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded) {
        Helpers.Helpers.TestResponse(withReferenceProperties + propertyName, FilePrefix + fileName, body, method, code);
    }

    protected void WithCollection(string propertyName, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded) {
        Helpers.Helpers.TestResponse(withCollectionProperties + propertyName, FilePrefix + fileName, body, method, code);
    }
}