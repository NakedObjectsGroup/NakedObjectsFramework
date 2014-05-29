// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that a property or all the properties of a class are to be eagerly rendered (or in the future loaded from the database).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class EagerlyAttribute : Attribute {
        public enum Do {
            Rendering
        }

        private readonly Do what;

        public EagerlyAttribute(Do what) {
            this.what = what;
        }
    }
}