// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Security {
    /// <summary>
    ///     Authorizes a list of roles (comma seperated) and/or users to view and use the property. If the Roles/Users parameters is present then
    ///     access is limited to those lists. If absent then no restriction is imposed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuthorizeActionAttribute : Attribute {
        public string Roles { get; set; }
        public string Users { get; set; }
    }
}