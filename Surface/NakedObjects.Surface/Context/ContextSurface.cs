// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public abstract class ContextSurface {
        public abstract string Id { get; }

        public virtual INakedObjectSurface Target { get; set; }

        public virtual string Reason { get; set; }

        public virtual Cause ErrorCause { get; set; }

        public virtual INakedObjectSurface ProposedNakedObject { get; set; }

        public virtual object ProposedValue { get; set; }

        public abstract INakedObjectSpecificationSurface Specification { get; }
    }
}