// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Facade.Contexts;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2.Contexts {
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

        protected T ToContextFacade<T>(T context, IFrameworkFacade facade) where T : ContextFacade {
            context.Target = Target == null ? (NakedTarget == null ? null : new ObjectFacade(NakedTarget, facade)) : new ObjectFacade(Target, facade);
            context.Reason = Reason;
            context.ErrorCause = ErrorCause;
            context.ProposedNakedObject = ProposedNakedObject == null ? null : new ObjectFacade(ProposedNakedObject, facade);
            context.ProposedValue = ProposedValue;

            return context;
        }
    }
}