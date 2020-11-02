// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace NakedObjects.Meta.Menu {
    [Serializable]
    public class MenuFactory : IMenuFactory {
        #region Injected ServicesManager

        private readonly IMetamodelBuilder metamodel;

        #endregion

        public MenuFactory(IMetamodelBuilder metamodel) => this.metamodel = metamodel;

        #region IMenuFactory Members

        public IMenu NewMenu<T>(bool addAllActions = false, string name = null) => new MenuImpl(metamodel, typeof(T), addAllActions, name);

        public IMenu NewMenu(Type type, bool addAllActions = false, string name = null) => new MenuImpl(metamodel, type, addAllActions, name);
        , 
       public IMenu NewMenu(string name, string id) => new MenuImpl(metamodel, name, id);
        public IMenu NewMenu(string name, string id, Type defaultType, bool addAllActions = false) => new MenuImpl(metamodel, defaultType, addAllActions, name, id);

        #endregion
    }
}