// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Specialized;
using System.Web;

namespace NakedObjects.Web.Mvc.Helpers
{
    public interface IEncryptDecrypt
    {
        string Encrypt(HttpSessionStateBase session, string toEncrypt);
        string Decrypt(HttpSessionStateBase session, string toDecrypt);

        Tuple<string, string> Encrypt(HttpSessionStateBase session, string name, string value);
        void Decrypt(HttpSessionStateBase session, NameValueCollection collection);
    }
}