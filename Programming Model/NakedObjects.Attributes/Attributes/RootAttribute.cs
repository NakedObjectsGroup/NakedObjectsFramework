// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     Experimental attribute that indicates that the annotated property should be injected
    ///     with the aggregate root. This is currently just used for Entity Framework complex types
    ///     - its use for other types of aggregated objects is under investigation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RootAttribute : Attribute {}
}