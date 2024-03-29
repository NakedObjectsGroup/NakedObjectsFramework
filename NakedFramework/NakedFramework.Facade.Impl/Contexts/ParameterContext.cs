// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Contexts;

public class ParameterContext : Context {
    public IActionParameterSpec Parameter { get; set; }
    public IActionSpec Action { get; set; }
    public ListContext Completions { get; set; }

    public override string Id => Parameter.Id;

    public override ITypeSpec Specification => Parameter.Spec;

    public string OverloadedUniqueId { get; init; }

    public ParameterContextFacade ToParameterContextFacade(IFrameworkFacade facade, INakedFramework framework, string menuId) {
        var pc = new ParameterContextFacade {
            Parameter = new ActionParameterFacade(Parameter, facade, framework),
            Action = new ActionFacade(Action, facade, framework),
            Completions = Completions?.ToListContextFacade(facade, framework),
            MenuId = menuId
        };
        return ToContextFacade(pc, facade, framework);
    }
}