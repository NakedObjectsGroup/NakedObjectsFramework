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
using NakedObjects.Facade.Impl.Utility;

namespace NakedObjects.Facade.Impl {
    public class ActionFacade : IActionFacade {
        private readonly INakedObjectsFramework framework;
        private readonly string overloadedUniqueId;

        public ActionFacade(IActionSpec action, IFrameworkFacade frameworkFacade, INakedObjectsFramework framework, string overloadedUniqueId) {
            FacadeUtils.AssertNotNull(action, "Action is null");
            FacadeUtils.AssertNotNull(framework, "framework is null");
            FacadeUtils.AssertNotNull(overloadedUniqueId, "overloadedUniqueId is null");
            FacadeUtils.AssertNotNull(frameworkFacade, "FrameworkFacade is null");

            WrappedSpec = action;
            this.framework = framework;
            this.overloadedUniqueId = overloadedUniqueId;
            FrameworkFacade = frameworkFacade;
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

        public IActionParameterFacade[] Parameters {
            get { return WrappedSpec.Parameters.Select(p => new ActionParameterFacade(p, FrameworkFacade, framework, overloadedUniqueId)).Cast<IActionParameterFacade>().ToArray(); }
        }

        public bool IsVisible(IObjectFacade objectFacade) {
            return WrappedSpec.IsVisible(((ObjectFacade) objectFacade).WrappedNakedObject);
        }

        public IConsentFacade IsUsable(IObjectFacade objectFacade) {
            return new ConsentFacade(WrappedSpec.IsUsable(((ObjectFacade) objectFacade).WrappedNakedObject));
        }

        public ITypeFacade OnType => new TypeFacade(WrappedSpec.OnSpec, FrameworkFacade, framework);

        public IFrameworkFacade FrameworkFacade { get; set; }

        public bool RenderEagerly => WrappedSpec.GetRenderEagerly();

        public Tuple<bool, string[]> TableViewData => WrappedSpec.GetTableViewData();

        public int PageSize => WrappedSpec.GetFacet<IPageSizeFacet>().Value;

        public string PresentationHint => WrappedSpec.GetPresentationHint();

        public int? NumberOfLines => WrappedSpec.GetNumberOfLines();

        #endregion

        public override bool Equals(object obj) {
            var nakedObjectActionWrapper = obj as ActionFacade;
            if (nakedObjectActionWrapper != null) {
                return Equals(nakedObjectActionWrapper);
            }
            return false;
        }

        public bool Equals(ActionFacade other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return Equals(other.WrappedSpec, WrappedSpec);
        }

        public override int GetHashCode() {
            return (WrappedSpec != null ? WrappedSpec.GetHashCode() : 0);
        }
    }
}