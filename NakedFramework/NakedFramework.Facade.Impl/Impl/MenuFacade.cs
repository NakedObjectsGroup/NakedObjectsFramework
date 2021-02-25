// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Menu;
using NakedFramework.Facade.Facade;

namespace NakedFramework.Facade.Impl.Impl {
    public class MenuFacade : IMenuFacade {
        public MenuFacade(IMenuImmutable wrapped, IFrameworkFacade facade, INakedObjectsFramework framework) {
            Wrapped = wrapped;
            MenuItems = wrapped.MenuItems.Select(i => Wrap(i, facade, framework)).ToList();
            Name = wrapped.Name;
            Id = wrapped.Id;
            Grouping = wrapped.Grouping;
        }

        #region IMenuFacade Members

        public object Wrapped { get; }
        public IList<IMenuItemFacade> MenuItems { get; }
        public string Name { get; }
        public string Id { get; }
        public string Grouping { get; }

        #endregion

        private static IMenuItemFacade Wrap(IMenuItemImmutable menu, IFrameworkFacade facade, INakedObjectsFramework framework) =>
            menu switch {
                IMenuActionImmutable immutable => new MenuActionFacade(immutable, facade, framework),
                IMenuImmutable menuImmutable => new MenuFacade(menuImmutable, facade, framework),
                _ => new MenuItemFacade(menu)
            };
    }
}