// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     The annotated member cannot be used in any instance of its class
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When applied to the property it means that the user may not modify
    ///         the value of that property (though it may still be modified programmatically).
    ///         When applied to an action method, it means that the user cannot invoke that method
    ///         <para>
    ///             This attribute can also take a single parameter indicating when it is to be disabled
    ///             <see cref="WhenTo" /> (defaulting to <see cref="WhenTo.Always" />)
    ///         </para>
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class DisabledAttribute : Attribute {
        public DisabledAttribute() {
            Value = WhenTo.Always;
        }

        public DisabledAttribute(WhenTo w) {
            Value = w;
        }

        public WhenTo Value { get; private set; }
    }
}