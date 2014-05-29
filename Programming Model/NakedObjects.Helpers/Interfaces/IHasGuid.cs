// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects {
    /// <summary>
    ///     This interface is intended for use with 'Interface Associations' and specifically
    ///     to work in conjunction with NakedObjects.Services.ObjectFinder.
    ///     If the class being associated implements IHasGuid, then the compound key will use this
    ///     Guid (together with the fully qualified type name) to form the compound key.  This has the
    ///     advantage that the Guid may be set up when the object is created rather than waiting until
    ///     the object is persisted (if the keys are database generated, that is). This is
    ///     important when defining interface associations between transient objects that are all
    ///     persisted in one transation.
    /// </summary>
    public interface IHasGuid {
        /// <summary>
        ///     The Guid property should be set up with a new Guid when the object is created,
        ///     for example in the Created() method.
        /// </summary>
        Guid Guid { get; set; }
    }
}