// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Override default location for execution of action when running client server.
    /// </summary>
    /// <seealso cref="Where" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExecutedAttribute : Attribute {
        public ExecutedAttribute(Where w) {
            IsAjax = false;
            Value = w;
        }

        public ExecutedAttribute(Ajax a) {
            IsAjax = true;
            AjaxValue = a;
        }

        public Where Value { get; private set; }
        public Ajax AjaxValue { get; private set; }
        public bool IsAjax { get; private set; }
    }
}