// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     provides information about the carriage returns in a <see cref="string" /> property or action parameter.
    ///     The attribute indicates that:
    ///     <list type="bullet">
    ///         <item>the String property or parameter may contain carriage returns</item>
    ///         <item>(optionally) the typical number of such carriage returns (default 6)</item>
    ///         <item>(optionally) the width of each line before wrapping (default 0)</item>
    ///     </list>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class MultiLineAttribute : Attribute {
        public MultiLineAttribute() {
            NumberOfLines = 6;
            Width = 0;
        }

        public int NumberOfLines { get; set; }

        public int Width { get; set; }
    }
}