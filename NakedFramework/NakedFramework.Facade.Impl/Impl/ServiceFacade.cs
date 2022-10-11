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
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Impl;

public class ServiceFacade : IServiceFacade {
    public ServiceFacade(IMenuImmutable wrapped, IFrameworkFacade facade, INakedFramework framework, ITypeFacade specification) {
        Wrapped = wrapped;
        ServiceItems = wrapped.MenuItems.Select(i => Wrap(i, facade, framework, specification)).ToList();
        Name = wrapped.Name;
        Id = wrapped.Id;
        Grouping = wrapped.Grouping;
        Specification = specification;
    }

    private static IServiceItemFacade Wrap(IMenuItemImmutable menu, IFrameworkFacade facade, INakedFramework framework, ITypeFacade specification) =>
        menu switch {
            IMenuActionImmutable immutable => new ServiceActionFacade(immutable, facade, framework),
            IMenuImmutable menuImmutable => new ServiceFacade(menuImmutable, facade, framework, specification),
            _ => new ServiceItemFacade(menu)
        };

    #region IMenuFacade Members

    public object Wrapped { get; }
    public IList<IServiceItemFacade> ServiceItems { get; }
    public ITypeFacade Specification { get;  }
    public string Name { get; }
    public string Id { get; }
    public string Grouping { get; }

    #endregion
}