// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Use for immutable objects where there is a bounded set of instances
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The number of instances is expected to be small enough that all instances can be held in memory.
    ///         The viewer will use this information to render all the instances of this class available to the user
    ///         in a convenient form, such as a drop-down list. Although this is not enforced, Bounded is intended
    ///         for use on sealed (non-overridable) classes. Its behaviour when used on interfaces, or classes with sub-classes
    ///         is not specified).
    ///     </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public class BoundedAttribute : Attribute {}
}