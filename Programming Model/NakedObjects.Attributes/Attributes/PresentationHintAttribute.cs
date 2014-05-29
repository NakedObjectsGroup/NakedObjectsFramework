// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     A hint added to the associated display element. For example a class on the html.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class PresentationHintAttribute : Attribute {
        public PresentationHintAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}