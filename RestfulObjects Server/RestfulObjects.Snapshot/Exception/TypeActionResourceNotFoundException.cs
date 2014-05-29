// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Surface;

namespace RestfulObjects.Snapshot.Utility {
    public class TypeActionResourceNotFoundException : ResourceNotFoundNOSException {
        public TypeActionResourceNotFoundException(string resourceId, string domainId) : base(resourceId) {
            DomainId = domainId;
        }

        public string DomainId { get; private set; }

        public override string Message {
            get { return String.Format("No such domain type action {0} in domain type {1}", ResourceId, DomainId); }
        }
    }
}