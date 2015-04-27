// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Menu;
using NakedObjects.Surface.Interface;

namespace NakedObjects.Surface.Nof4.Wrapper {
    public class MenuActionWrapper : IMenuAction, IMenuItem {
        private readonly IMenuActionImmutable wrapped;

        public MenuActionWrapper(IMenuActionImmutable wrapped) {
            this.wrapped = wrapped;
        }

        #region IMenuAction Members

        public INakedObjectActionSurface Action { get; private set; }

        #endregion

        #region IMenuItem Members

        public string Name { get; private set; }
        public string Id { get; private set; }

        #endregion
    }
}