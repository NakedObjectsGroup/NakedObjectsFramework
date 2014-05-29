// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    ///     Indicates that a request to resolve an object has failed. Unresolved objects should never be used as they
    ///     will cause further errors.
    /// </summary>
    public class ResolveException : NakedObjectSystemException {
        public ResolveException(string msg)
            : base(msg) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}