// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedFramework.Architecture.Framework;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Facade.Impl.Contexts {
    public class MenuContext {
        public IMenuImmutable[] List { get; set; }
        public IObjectSpec ElementType { get; set; }

        public MenuContextFacade ToMenuContextFacade(IFrameworkFacade facade, INakedObjectsFramework framework) {
            return new MenuContextFacade {
                ElementType = new TypeFacade(ElementType, facade, framework),
                List = List.Select(m => new MenuFacade(m, facade, framework)).Cast<IMenuFacade>().ToArray()
            };
        }
    }
}