// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Context {
    public class ActionContext : Context {
        private ParameterContext[] parameters;

        public INakedObjectAction Action { get; set; }

        public override string Id {
            get { return Action.Id; }
        }

        public override INakedObjectSpecification Specification {
            get { return Action.ReturnType; }
        }

        public ParameterContext[] VisibleParameters {
            get { return parameters ?? new ParameterContext[] {}; }
            set { parameters = value; }
        }

        public string OverloadedUniqueId { get; set; }

        public ActionContextSurface ToActionContextSurface(INakedObjectsSurface surface) {
            var ac = new ActionContextSurface {
                Action = new NakedObjectActionWrapper(Action, surface, OverloadedUniqueId ?? ""),
                VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextSurface(surface)).ToArray()
            };
            return ToContextSurface(ac, surface);
        }
    }
}