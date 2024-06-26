// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Facade.Contexts;
using NakedFramework.Facade.Impl.Impl;
using NakedFramework.Facade.Interface;

namespace NakedFramework.Facade.Impl.Contexts;

public class ActionContext : Context {
    private ParameterContext[] parameters;

    private PropertyContext[] properties;

    public IActionSpec Action { get; init; }

    public override string Id => Action.Id;

    public override ITypeSpec Specification => Action.ReturnSpec;

    public ParameterContext[] VisibleParameters {
        get => parameters ?? Array.Empty<ParameterContext>();
        set => parameters = value;
    }

    public PropertyContext[] VisibleProperties {
        get => properties ?? Array.Empty<PropertyContext>();
        set => properties = value;
    }

    public string MenuPath { get; init; }

    public string MenuId { get; init; }
    public bool IsStaticService { get; set; }

    public ActionContextFacade ToActionContextFacade(IFrameworkFacade facade, INakedFramework framework) {
        var ac = new ActionContextFacade {
            MenuPath = MenuPath,
            MenuId = MenuId,
            IsStaticService = IsStaticService,
            Action = new ActionFacade(Action, facade, framework),
            VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextFacade(facade, framework, MenuId)).ToArray(),
            VisibleProperties = VisibleProperties.Select(p => p.ToPropertyContextFacade(facade, framework)).ToArray()
        };
        return ToContextFacade(ac, facade, framework);
    }
}