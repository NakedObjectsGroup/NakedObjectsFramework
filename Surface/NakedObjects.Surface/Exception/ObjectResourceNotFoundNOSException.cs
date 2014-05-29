// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class ObjectResourceNotFoundNOSException : ResourceNotFoundNOSException {
        public ObjectResourceNotFoundNOSException(string resourceId, Exception e) : base(resourceId, e) {}
        public ObjectResourceNotFoundNOSException(string resourceId) : base(resourceId) {}

        public override string Message {
            get { return String.Format("No such domain object {0}", ResourceId); }
        }
    }
}