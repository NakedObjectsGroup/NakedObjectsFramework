// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using NakedObjects.Facade.Nof2;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public class ParameterContext : Context {
        public NakedObjectActionParameter Parameter { get; set; }
        public ActionWrapper Action { get; set; }

        public override string Id {
            get { return Parameter.getId(); }
        }

        public override NakedObjectSpecification Specification {
            get { return Parameter.getSpecification(); }
        }

        public ParameterContextFacade ToParameterContextSurface(IFrameworkFacade surface) {
            var pc = new ParameterContextFacade {
                                                     Parameter = new ActionParameterFacade(Parameter, Target, surface),
                                                     Target = new ObjectFacade(Target, surface),
                                                     Action = new ActionFacade(Action, Target, surface)
                                                 };
            return ToContextSurface(pc, surface);
        }
    }
}