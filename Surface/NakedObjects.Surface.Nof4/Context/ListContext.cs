using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Context;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Context {
    public class ListContext {
        public INakedObject[] List { get; set; }
        public INakedObjectSpecification ElementType { get; set; }
        public bool IsListOfServices { get; set; }

        public ListContextSurface ToListContextSurface(INakedObjectsSurface surface, INakedObjectsFramework framework) {
            return new ListContextSurface {
                ElementType = new NakedObjectSpecificationWrapper(ElementType, surface, framework),
                List = List.Select(no => NakedObjectWrapper.Wrap(no, surface, framework)).Cast<INakedObjectSurface>().ToArray(),
                IsListOfServices = IsListOfServices
            };
        }
    }
}