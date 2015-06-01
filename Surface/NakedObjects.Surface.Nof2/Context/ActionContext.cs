// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof2.Wrapper;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public class ActionContext : Context {
        private ParameterContext[] parameters;
        public ActionWrapper Action { get; set; }

        public override string Id {
            get { return Action.getId(); }
        }

        public override NakedObjectSpecification Specification {
            get { return Action.getReturnType(); }
        }

        public ParameterContext[] VisibleParameters {
            get { return parameters ?? new ParameterContext[] {}; }
            set { parameters = value; }
        }

        public ActionContextSurface ToActionContextSurface(IFrameworkFacade surface) {
            var ac = new ActionContextSurface {
                                                  Action = new NakedObjectActionWrapper(Action, Target, surface),
                                                  VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextSurface(surface)).ToArray()
                                              };

            return ToContextSurface(ac, surface);
        }
    }
}