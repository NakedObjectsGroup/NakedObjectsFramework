// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class PropertyContextSurface : ContextSurface {
        public INakedObjectAssociationSurface Property { get; set; }

        public bool Mutated { get; set; }

        public override string Id {
            get { return Property.Id; }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return Property.Specification; }
        }
    }
}