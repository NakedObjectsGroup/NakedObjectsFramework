// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Used when you want to specify the way something is named on the user interface i.e. when you do
    ///     not want to use the name generated automatically by the system. It can be applied to objects,
    ///     members (properties, collections, and actions) and to parameters within an action method.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Instead of this may use <see cref="System.ComponentModel.DisplayNameAttribute" /> but note that it is not applicable to interfaces
    ///         or parameters.
    ///     </para>
    /// </remarks>
    /// <seealso cref="PluralAttribute" />
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class NamedAttribute : Attribute {
        public NamedAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}