// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class ActionResultContextSurface : ContextSurface {
        public ObjectContextSurface Result { get; set; }

        public bool HasResult { get; set; }

        public ActionContextSurface ActionContext { get; set; }

        public override INakedObjectSurface Target {
            get { return ActionContext.Target; }
        }

        public override string Id {
            get { return ActionContext.Action.Id; }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return Result == null ? ActionContext.Specification : Result.Specification; }
        }
    }
}