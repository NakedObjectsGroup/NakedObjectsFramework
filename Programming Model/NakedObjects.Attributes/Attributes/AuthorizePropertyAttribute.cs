// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Security {
    /// <summary>
    ///     Authorizes a list of roles (comma seperated) and/or users to view and/or edit the property. If the EditRoles/Users or ViewRoles/Users parameter is present then
    ///     access is limited to that list. If it is absent then no restriction is imposed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class AuthorizePropertyAttribute : Attribute {
        public string ViewRoles { get; set; }
        public string EditRoles { get; set; }
        public string ViewUsers { get; set; }
        public string EditUsers { get; set; }
    }
}