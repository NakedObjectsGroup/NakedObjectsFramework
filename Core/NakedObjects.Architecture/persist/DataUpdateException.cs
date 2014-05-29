// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Persist {
    public class DataUpdateException : ObjectPersistenceException {
        private readonly IOid sourceOid;

        public DataUpdateException(INakedObject nakedObject, IVersion updated)
            : this(string.Format(Resources.NakedObjects.DataUpdateMessage, nakedObject.Version.User, nakedObject.TitleString(), DateTime.Now.ToLongTimeString(), Environment.NewLine, Environment.NewLine, nakedObject.Version, updated), nakedObject.Oid) {}

        public DataUpdateException(string message, IOid source)
            : base(message) {
            sourceOid = source;
        }

        public DataUpdateException(string message, Exception cause)
            : base(message, cause) {}

        public virtual IOid SourceOid {
            get { return sourceOid; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}