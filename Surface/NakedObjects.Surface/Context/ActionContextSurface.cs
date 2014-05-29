// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class ActionContextSurface : ContextSurface {
        public INakedObjectActionSurface Action { get; set; }

        public override string Id {
            get { return Action.Id; }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return Action.ReturnType; }
        }

        public ParameterContextSurface[] VisibleParameters { get; set; }
    }
}