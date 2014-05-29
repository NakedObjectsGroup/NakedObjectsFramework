// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class ChoiceContextSurface : ContextSurface {
        private readonly string id;
        private readonly INakedObjectSpecificationSurface spec;

        public ChoiceContextSurface(string id, INakedObjectSpecificationSurface spec) {
            this.id = id;
            this.spec = spec;
        }

        public override string Id {
            get { return id; }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return spec; }
        }
    }
}