// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Metamodel.Profile;

[assembly: InternalsVisibleTo("NakedFunctions.Reflector")]
[assembly: InternalsVisibleTo("NakedFramework.Persistor.EF6")]
[assembly: InternalsVisibleTo("NakedFramework.Persistor.EFCore")]
[assembly: InternalsVisibleTo("NakedLegacy.Reflector")]

namespace NakedFramework.DependencyInjection.Extensions;

public class NakedFrameworkOptions {
    public NakedFrameworkOptions(IServiceCollection services) => Services = services;
    public IAuthorizationConfiguration AuthorizationConfiguration { get; set; }
    public IAuditConfiguration AuditConfiguration { get; set; }
    public IProfileConfiguration ProfileConfiguration { get; set; }
    public bool UseI18N { get; set; } = false;
    public Func<IMenuFactory, IMenu[]> MainMenus { get; set; }
    public Func<Type[], Type[]> SupportedSystemTypes { get; set; } = t => t;
    public IServiceCollection Services { get; }
    public int HashMapCapacity { get; set; } = 10;
    internal Type[] AdditionalSystemTypes { get; set; } = Array.Empty<Type>();
    internal Type[] AdditionalUnpersistedTypes { get; set; } = Array.Empty<Type>();
}