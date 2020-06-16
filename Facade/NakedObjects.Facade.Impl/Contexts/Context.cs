// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Facade.Impl.Contexts {
    public abstract class Context {
        public abstract string Id { get; }
        public INakedObjectAdapter Target { get; set; }
        public string Reason { get; set; }
        public Cause ErrorCause { get; set; }
        public INakedObjectAdapter ProposedNakedObject { get; set; }
        public object ProposedValue { get; set; }
        public abstract ITypeSpec Specification { get; }

        protected T ToContextFacade<T>(T context, IFrameworkFacade facade, INakedObjectsFramework framework) where T : ContextFacade {
            context.Target = ObjectFacade.Wrap(Target, facade, framework);
            context.Reason = Reason;
            context.ErrorCause = ErrorCause;
            context.ProposedObjectFacade = ObjectFacade.Wrap(ProposedNakedObject, facade, framework);
            context.ProposedValue = ProposedValue;
            context.Warnings = framework.MessageBroker.Warnings;
            context.Messages = framework.MessageBroker.Messages;

            return context;
        }
    }
}