// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Do not allow the state of annotated objects to be changed through the user interface.
    ///     It should be considered a programmer error to  do so programmatically
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ImmutableAttribute : Attribute {
        public ImmutableAttribute() {
            Value = WhenTo.Always;
        }

        public ImmutableAttribute(WhenTo w) {
            Value = w;
        }

        public WhenTo Value { get; private set; }
    }
}