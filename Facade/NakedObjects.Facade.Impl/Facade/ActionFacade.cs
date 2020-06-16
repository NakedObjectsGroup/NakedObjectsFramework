// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class ActionFacade : IActionFacade {
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;

        public ActionFacade(IActionSpec action, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework, string overloadedUniqueId) {
            WrappedSpec = action ?? throw new NullReferenceException($"{nameof(action)} is null");
            this.framework = framework ?? throw new NullReferenceException($"{nameof(framework)} is null");
            this.overloadedUniqueId = overloadedUniqueId ?? throw new NullReferenceException($"{nameof(overloadedUniqueId)} is null");
            FrameworkFacade = frameworkFacade ?? throw new NullReferenceException($"{nameof(frameworkFacade)} is null");
        }

        public IActionSpec WrappedSpec { get; }

        public ITypeFacade Specification => new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

        #region IActionFacade Members

        public bool IsContributed => WrappedSpec.IsContributedMethod;

        public string Name => WrappedSpec.Name;

        public string Description => WrappedSpec.Description;

        public bool IsQueryOnly => WrappedSpec.ReturnSpec.IsQueryable || WrappedSpec.ContainsFacet<IQueryOnlyFacet>();

        public bool IsIdempotent => WrappedSpec.ContainsFacet<IIdempotentFacet>();

        public int MemberOrder => WrappedSpec.GetMemberOrder();

        public string MemberOrderName => WrappedSpec.GetMemberOrderName();

        public string Id => WrappedSpec.Id + overloadedUniqueId;

        public ITypeFacade ReturnType => new TypeFacade(WrappedSpec.ReturnSpec, FrameworkFacade, framework);

        public ITypeFacade ElementType {
            get {
                var elementSpec = WrappedSpec.ElementSpec;
                return elementSpec == null ? null : new TypeFacade(elementSpec, FrameworkFacade, framework);
            }
        }

        public int ParameterCount => WrappedSpec.ParameterCount;

        public IActionParameterFacade[] Parameters => WrappedSpec.Parameters.Select(p => new ActionParameterFacade(p, FrameworkFacade, framework, overloadedUniqueId)).Cast<IActionParameterFacade>().ToArray();

        public bool IsVisible(IObjectFacade objectFacade) => WrappedSpec.IsVisible(((ObjectFacade) objectFacade).WrappedNakedObject);

        public IConsentFacade IsUsable(IObjectFacade objectFacade) => new ConsentFacade(WrappedSpec.IsUsable(((ObjectFacade) objectFacade).WrappedNakedObject));

        public ITypeFacade OnType => new TypeFacade(WrappedSpec.OnSpec, FrameworkFacade, framework);

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool RenderEagerly => WrappedSpec.GetRenderEagerly();

        public (bool, string[])? TableViewData => WrappedSpec.GetTableViewData();

        public int PageSize => WrappedSpec.GetFacet<IPageSizeFacet>().Value;

        public string PresentationHint => WrappedSpec.GetPresentationHint();

        public int? NumberOfLines => WrappedSpec.GetNumberOfLines();

        #endregion

        public override bool Equals(object obj) => obj is ActionFacade af && Equals(af);

        public bool Equals(ActionFacade other) {
            if (ReferenceEquals(null, other)) { return false; }

            return ReferenceEquals(this, other) || Equals(other.WrappedSpec, WrappedSpec);
        }

        public override int GetHashCode() => WrappedSpec != null ? WrappedSpec.GetHashCode() : 0;
    }
}