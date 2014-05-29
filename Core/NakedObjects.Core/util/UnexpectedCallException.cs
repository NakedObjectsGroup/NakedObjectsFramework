// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture;

namespace NakedObjects.Core.Util {
    /// <summary>
    ///     Indicates that a call was made to a method (normally an overridden one) that was not expected, and hence
    ///     not coded for.
    /// </summary>
    public class UnexpectedCallException : NakedObjectSystemException {
        public UnexpectedCallException()
            : base("This method call was not expected") {}

        public UnexpectedCallException(string arg0)
            : base(arg0) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}