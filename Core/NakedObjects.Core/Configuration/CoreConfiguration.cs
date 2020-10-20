// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Menu;

namespace NakedObjects.Core.Configuration {
    public class CoreConfiguration : ICoreConfiguration {
        public CoreConfiguration((Type rootType, string name, bool allActions, Action<IMenu> customConstruction)[] mainMenus = null) => MainMenus = mainMenus;

        public (Type rootType, string name, bool allActions, Action<IMenu> customConstruction)[] MainMenus { get; }

        public Func<IMenuFactory, IMenu[]> GetMainMenus() =>
            mf => {
                var mainMenus = MainMenus;
                IMenu[] menus = null;

                if (mainMenus is not null && mainMenus.Any())
                {
                    menus = mainMenus.Select(tuple => {
                        var (type, name, addAll, action) = tuple;
                        var menu = mf.NewMenu(type, addAll, name);
                        action?.Invoke(menu);
                        return menu;
                    }).ToArray();
                }

                return menus;
            };
    }
}