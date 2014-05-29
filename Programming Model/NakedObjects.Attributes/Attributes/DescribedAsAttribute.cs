// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Used to provide a short description of something that features on the user interface.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         How this description is used will depend upon the viewing mechanism
    ///         - but it may be thought of as being like a 'tool tip'. Descriptions may be provided for objects,
    ///         members (properties, collections and actions), and for individual parameters within an action method.
    ///         DescribedAs therefore works in a very similar manner to <see cref="NamedAttribute" />.
    ///     </para>
    ///     <para>
    ///         Instead of this may use <see cref="System.ComponentModel.DescriptionAttribute" />
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public class DescribedAsAttribute : Attribute {
        public DescribedAsAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}