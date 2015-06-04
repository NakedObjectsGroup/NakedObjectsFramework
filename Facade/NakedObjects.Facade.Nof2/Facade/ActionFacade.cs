// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Facade.Nof2.Utility;
using org.nakedobjects.@object;

namespace NakedObjects.Facade.Nof2 {
    public class ActionFacade : IActionFacade {
        private readonly ActionWrapper action;
        private readonly Naked target;

        public ActionFacade(ActionWrapper action, Naked target, IFrameworkFacade surface) {
            this.action = action;
            this.target = target;
            Surface = surface;
        }

        public ITypeFacade Specification {
            get {
                NakedObjectSpecification rt = action.getReturnType();
                return rt == null ? (ITypeFacade) new VoidTypeFacade() : new TypeFacade(action.getReturnType(), target, Surface);
            }
        }

        protected int MemberOrder {
            get { return 0; }
        }

        #region IActionFacade Members

        public bool IsContributed {
            get { return false; }
        }

        int IActionFacade.MemberOrder {
            get { return MemberOrder; }
        }

        public IDictionary<string, object> ExtensionData { get; private set; }
        public Tuple<bool, string[]> TableViewData { get; private set; }
        public bool RenderEagerly { get; private set; }
        public int PageSize { get; private set; }

        public string Name {
            get { return action.getName(); }
        }

        public string Description {
            get { return action.getDescription(); }
        }

        public bool IsQueryOnly {
            get { return false; }
        }

        public bool IsIdempotent {
            get { return false; }
        }

        public string Id {
            get { return action.getId(); }
        }

        public ITypeFacade ReturnType {
            get { return Specification; }
        }

        public ITypeFacade ElementType {
            get { return new TypeFacade(org.nakedobjects.@object.NakedObjects.getSpecificationLoader().loadSpecification(typeof (object).FullName), null, Surface); }
        }

        public int ParameterCount {
            get { return action.getParameterCount(); }
        }

        public IActionParameterFacade[] Parameters {
            get { return action.GetParameters((NakedReference) target).Select(p => new ActionParameterFacade(p, target, Surface)).Cast<IActionParameterFacade>().ToArray(); }
        }

        public bool IsVisible(IObjectFacade nakedObject) {
            return action.isVisible((NakedReference) (((ObjectFacade) nakedObject).NakedObject)).isAllowed();
        }

        public IConsentFacade IsUsable(IObjectFacade nakedObject) {
            return new ConsentFacade(action.isAvailable((NakedReference) (((ObjectFacade) nakedObject).NakedObject)));
        }

        public ITypeFacade OnType {
            get { throw new NotImplementedException(); }
        }

        public string PresentationHint { get; private set; }
        public IFrameworkFacade Surface { get; set; }

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
            return Equals(other.action, action);
        }

        public override int GetHashCode() {
            return (action != null ? action.GetHashCode() : 0);
        }
    }
}