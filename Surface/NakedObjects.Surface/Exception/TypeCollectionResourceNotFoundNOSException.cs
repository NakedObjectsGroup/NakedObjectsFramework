// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class TypeCollectionResourceNotFoundNOSException : ResourceNotFoundNOSException {
        public TypeCollectionResourceNotFoundNOSException(string resourceId, string domainId) : base(resourceId) {
            DomainId = domainId;
        }

        public string DomainId { get; private set; }

        public override string Message {
            get { return String.Format("No such domain collection {0} in domain type {1}", ResourceId, DomainId); }
        }
    }
}