// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Used when you want to specify a name for the icon.
    /// </summary>
    /// <seealso cref="PluralAttribute" />
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class IconNameAttribute : Attribute {
        public IconNameAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}