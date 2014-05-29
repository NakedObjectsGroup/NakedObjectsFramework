// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that the member (property, collection or action) to which it is applied should never be
    ///     visible to the user
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class HiddenAttribute : Attribute {
        public HiddenAttribute() {
            Value = WhenTo.Always;
        }

        public HiddenAttribute(WhenTo w) {
            Value = w;
        }

        public WhenTo Value { get; private set; }
    }
}