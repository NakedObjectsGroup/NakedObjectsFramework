// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class PropertyResourceNotFoundNOSException : ResourceNotFoundNOSException {
        public PropertyResourceNotFoundNOSException(string resourceId) : base(resourceId) {}

        public override string Message {
            get { return String.Format(String.Format("No such property {0}", ResourceId)); }
        }
    }
}