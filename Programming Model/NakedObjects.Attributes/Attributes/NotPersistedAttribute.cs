// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     This attribute indicates that transient instances of this class may be created but may not be
    ///     persisted, or that properties within a class are not persisted.
    ///     Attempting to persist such an object programmatically would throw an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NotPersistedAttribute : Attribute {}
}