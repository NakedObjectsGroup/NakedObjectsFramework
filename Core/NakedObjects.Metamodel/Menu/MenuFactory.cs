// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace NakedObjects.Meta.Menu {
    public class MenuFactory : IMenuFactory {
        #region Injected Services

        private readonly IMetamodelBuilder metamodel;

        #endregion

        public MenuFactory(IMetamodelBuilder metamodel) {
            this.metamodel = metamodel;
        }

        #region IMenuFactory Members

        public IMenu NewMenu(string name) {
            return new MenuImpl(metamodel, name);
        }

        public ITypedMenu<TObject> NewMenu<TObject>(bool addAllActions, string name = null) {
            return new TypedMenu<TObject>(metamodel, addAllActions, name);
        }

        #endregion
    }
}