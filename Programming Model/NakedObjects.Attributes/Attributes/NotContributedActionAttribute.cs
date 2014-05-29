// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Never allow this action to be contributed to an object menu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class NotContributedActionAttribute : Attribute {
        public NotContributedActionAttribute(params Type[] notContributedToTypes) {
            NotContributedToTypes = notContributedToTypes ?? new Type[] {};
        }

        public Type[] NotContributedToTypes { get; private set; }
    }
}