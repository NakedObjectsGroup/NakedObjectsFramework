// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     This attribute indicates that transient instances of this class will only be persisted programmatically. When the user
    ///     indicates that a transient object is to be saved the state of the object will be updated, but the object will not yet be
    ///     added to the persistent store.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ProgramPersistableOnlyAttribute : Attribute {}
}