// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     serves to validate, and potentially to normalise, the format of the input. The characters that can
    ///     be used are based on Microsoft's MaskedEdit control
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class MaskAttribute : Attribute {
        public MaskAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}