// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Menu;

namespace NakedObjects.Facade.Impl {
    public class MenuActionFacade : IMenuActionFacade, IMenuItemFacade {
        public MenuActionFacade(IMenuActionImmutable wrapped, IFrameworkFacade facade, INakedObjectsFramework framework) {
            Wrapped = wrapped;
            Name = wrapped.Name;
            Id = wrapped.Id;
            var action = framework.MetamodelManager.GetActionSpec(wrapped.Action);
            Action = new ActionFacade(action, facade, framework, "");
        }

        #region IMenuActionFacade Members

        public IActionFacade Action { get; }

        #endregion

        #region IMenuItemFacade Members

        public string Name { get; }
        public string Id { get; }
        public object Wrapped { get; }

        #endregion
    }
}