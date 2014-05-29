// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Indicates the maximum number of objects displayed in each page when a collection results from the action.
    ///     '0' will result in no paging.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class PageSizeAttribute : Attribute {
        public PageSizeAttribute(int i) {
            Value = i;
        }

        public int Value { get; private set; }
    }
}