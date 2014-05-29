// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Surface {
    public class ParameterContextSurface : ContextSurface {
        public INakedObjectActionParameterSurface Parameter { get; set; }

        public override string Id {
            get { return Parameter.Id; }
        }

        public override INakedObjectSpecificationSurface Specification {
            get { return Parameter.Specification; }
        }

        public INakedObjectActionSurface Action { get; set; }
    }
}