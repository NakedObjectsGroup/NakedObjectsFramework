// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class ServiceResourceNotFoundNOSException : ResourceNotFoundNOSException {
        public ServiceResourceNotFoundNOSException(string resourceId, Exception e) : base(resourceId, e) {}
        public ServiceResourceNotFoundNOSException(string resourceId) : base(resourceId) {}

        public override string Message {
            get { return String.Format("No such service {0}", ResourceId); }
        }
    }
}