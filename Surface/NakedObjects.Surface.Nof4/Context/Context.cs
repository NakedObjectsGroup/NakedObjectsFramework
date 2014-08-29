// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Surface.Nof4.Wrapper;

namespace NakedObjects.Surface.Nof4.Context {
    public abstract class Context {
        public abstract string Id { get; }

        public INakedObject Target { get; set; }

        public string Reason { get; set; }

        public Cause ErrorCause { get; set; }

        public INakedObject ProposedNakedObject { get; set; }

        public object ProposedValue { get; set; }

        public abstract INakedObjectSpecification Specification { get; }

        protected T ToContextSurface<T>(T context, INakedObjectsSurface surface, INakedObjectsFramework framework) where T : ContextSurface {
            context.Target = NakedObjectWrapper.Wrap(Target, surface, framework);
            context.Reason = Reason;
            context.ErrorCause = ErrorCause;
            context.ProposedNakedObject = NakedObjectWrapper.Wrap(ProposedNakedObject, surface, framework);
            context.ProposedValue = ProposedValue;

            return context;
        }
    }
}