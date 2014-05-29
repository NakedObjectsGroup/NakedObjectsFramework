// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Specific the order of properties in one place in the class
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The recommended mechanism for specifying the order in which properties are listed to the user is
    ///         <see
    ///             cref="MemberOrderAttribute" />
    ///         .
    ///         FieldOrder provides an alternative mechanism, in which the order is specified in one place in the
    ///         class, with the added advantage (currently) that you can easily specify groupings (which may be rendered by the
    ///         framework as sub-menus). However, FieldOrder is more 'brittle' to change: if you change the name of an existing
    ///         property you will need to ensure that the corresponding name within the FieldOrder attribute is also changed.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class FieldOrderAttribute : Attribute {
        public FieldOrderAttribute(string s) {
            Value = s;
        }

        public string Value { get; private set; }
    }
}