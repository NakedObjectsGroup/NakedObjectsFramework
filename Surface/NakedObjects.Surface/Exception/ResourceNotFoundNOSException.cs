// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public abstract class ResourceNotFoundNOSException : NakedObjectsSurfaceException {
        protected ResourceNotFoundNOSException(string resourceId, Exception e) : base(e) {
            ResourceId = resourceId;
        }

        protected ResourceNotFoundNOSException(string resourceId) {
            ResourceId = resourceId;
        }

        public string ResourceId { get; set; }
    }
}