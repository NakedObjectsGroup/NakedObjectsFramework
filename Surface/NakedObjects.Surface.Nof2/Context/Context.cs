// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Facade;
using NakedObjects.Surface;
using NakedObjects.Facade.Nof2;
using org.nakedobjects.@object;

namespace NakedObjects.Surface.Nof2.Context {
    public abstract class Context {
        private string reason;
        public abstract string Id { get; }

        public NakedObject Target { get; set; }
        public Naked NakedTarget { get; set; }

        public string Reason {
            get {
                // remove any @nnnn strings from reason 
                int i = reason == null ? -1 : reason.IndexOf('@');

                if (i > -1) {
                    string s = reason.Substring(0, i);
                    string ss = reason.Substring(i, reason.Length - (i));
                    int ni = ss.IndexOf(' ');
                    ss = ss.Substring(ni, ss.Length - ni);

                    return s + ss;
                }
                return reason;
            }
            set { reason = value; }
        }

        public Cause ErrorCause { get; set; }

        public Naked ProposedNakedObject { get; set; }

        public object ProposedValue { get; set; }

        public abstract NakedObjectSpecification Specification { get; }

        protected T ToContextSurface<T>(T context, IFrameworkFacade surface) where T : ContextSurface {
            context.Target = Target == null ? (NakedTarget == null ? null : new ObjectFacade(NakedTarget, surface)) : new ObjectFacade(Target, surface);
            context.Reason = Reason;
            context.ErrorCause = ErrorCause;
            context.ProposedNakedObject = ProposedNakedObject == null ? null : new ObjectFacade(ProposedNakedObject, surface);
            context.ProposedValue = ProposedValue;

            return context;
        }
    }
}