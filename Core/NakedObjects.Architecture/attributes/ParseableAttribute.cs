// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Not yet fully supported
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ParseableAttribute : Attribute {
        public ParseableAttribute() {
            ParserName = "";
            ParserClass = null;
        }

        public string ParserName { get; set; }

        public Type ParserClass { get; set; }
    }
}