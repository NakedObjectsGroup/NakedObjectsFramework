// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class PropertyTypeContextSurface {
        public INakedObjectAssociationSurface Property { get; set; }
        public INakedObjectSpecificationSurface OwningSpecification { get; set; }
    }
}