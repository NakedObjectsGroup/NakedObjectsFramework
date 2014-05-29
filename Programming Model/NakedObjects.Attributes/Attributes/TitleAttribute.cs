// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Identifies a property as the title for an object
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When the title for an object is simply the state of a property then this attribute allows that property to
    ///         be marked so that it is used insteat of providing an additional title method.
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class TitleAttribute : Attribute {}
}