// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class TypeActionParameterResourceNotFoundNOSException : ResourceNotFoundNOSException {
        public TypeActionParameterResourceNotFoundNOSException(string resourceId, string domainId, string parmId) : base(resourceId) {
            DomainId = domainId;
            ParmId = parmId;
        }

        public string DomainId { get; private set; }

        public string ParmId { get; private set; }

        public override string Message {
            get { return String.Format("No such parameter name", ResourceId, DomainId); }
        }
    }
}