// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Resources;

namespace NakedObjects.Architecture {
    /// <summary>
    ///     Indicates that a value entry is not valid. The entry may still parse correctly,
    ///     but it does not fulfil other other entry requirements
    /// </summary>
    public class InvalidEntryException : NakedObjectApplicationException {
        public InvalidEntryException()
            : this(ProgrammingModel.InvalidValue) {}

        public InvalidEntryException(string message)
            : base(message) {}

        public InvalidEntryException(Exception cause)
            : this(ProgrammingModel.InvalidValue, cause) {}

        public InvalidEntryException(string message, Exception cause)
            : base(message, cause) {}
    }
}