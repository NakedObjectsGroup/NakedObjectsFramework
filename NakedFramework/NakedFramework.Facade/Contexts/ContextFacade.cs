// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Facade.Facade;

namespace NakedFramework.Facade.Contexts {
    public abstract class ContextFacade {
        public abstract string Id { get; }
        public virtual IObjectFacade Target { get; set; }
        public virtual string Reason { get; set; }
        public virtual Cause ErrorCause { get; set; }
        public virtual IObjectFacade ProposedObjectFacade { get; set; }
        public virtual object ProposedValue { get; set; }
        public abstract ITypeFacade Specification { get; }
        public abstract ITypeFacade ElementSpecification { get; }
        public virtual Func<string[]> Warnings { get; set; }
        public virtual Func<string[]> Messages { get; set; }
    }
}