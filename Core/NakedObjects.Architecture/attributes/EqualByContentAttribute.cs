// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Not yet fully supported
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class EqualByContentAttribute : Attribute {
        public EqualByContentAttribute() {
            Value = WhenTo.Always;
        }

        public WhenTo Value { get; private set; }
    }
}