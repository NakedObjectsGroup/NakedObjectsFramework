using System;

namespace NakedObjects.Surface.Context {
    public class ListContextSurface {
        public INakedObjectSpecificationSurface ElementType { get; set; }
        public INakedObjectSurface[] List { get; set; }
        public bool IsListOfServices { get; set; }
    }
}