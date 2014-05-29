// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Persist {
    public class ConcurrencyException : ObjectPersistenceException {
        public ConcurrencyException(INakedObject nakedObject, IVersion updated)
            : this(Resources.NakedObjects.ConcurrencyMessage, nakedObject.Oid) {
            SourceNakedObject = nakedObject;
        }

        public ConcurrencyException(string message, IOid source)
            : base(message) {
            SourceOid = source;
        }

        public ConcurrencyException(string message, Exception cause)
            : base(message, cause) {}

        public IOid SourceOid { get; private set; }

        public INakedObject SourceNakedObject { get; set; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}