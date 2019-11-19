// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace RestfulObjects.Test.EndToEnd {
    /// <summary>
    /// 
    /// </summary>
    public abstract class ObjectAbstract {
        protected static string objectsUrl = Urls.Objects;

        protected virtual string FilePrefix {
            get { return "NO PREFIX SPECIFIED"; }
        }

        protected void Object(string oid, string fileName, string body = null, string method = Methods.Get, Codes code = Codes.Succeeded, string acceptHeader = null) {
            if (fileName != null) {
                fileName = FilePrefix + fileName;
            }
            Helpers.TestResponse(objectsUrl + oid, fileName, body, method, code, acceptHeader);
        }
    }
}