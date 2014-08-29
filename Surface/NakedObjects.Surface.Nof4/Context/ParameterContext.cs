// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Context {
    public class ParameterContext : Context {
        public INakedObjectActionParameter Parameter { get; set; }

        public INakedObjectAction Action { get; set; }

        public override string Id {
            get { return Parameter.Id; }
        }

        public override INakedObjectSpecification Specification {
            get { return Parameter.Specification; }
        }

        public string OverloadedUniqueId { get; set; }

        public ParameterContextSurface ToParameterContextSurface(INakedObjectsSurface surface, INakedObjectsFramework framework) {
            var pc = new ParameterContextSurface {
                Parameter = new NakedObjectActionParameterWrapper(Parameter, surface, framework, OverloadedUniqueId ?? ""),
                Action = new NakedObjectActionWrapper(Action, surface, framework, OverloadedUniqueId ?? "")
            };
            return ToContextSurface(pc, surface, framework);
        }
    }
}