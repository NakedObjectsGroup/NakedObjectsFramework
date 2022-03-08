// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Authorization;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Utils;
using NakedFunctions.Security;

namespace NakedFunctions.Reflector.Authorization;

[Serializable]
public sealed class AuthorizationManager : AbstractAuthorizationManager {
    private readonly ImmutableDictionary<Type, Func<object, object, string, IContext, bool>> isVisibleDelegates;

    public AuthorizationManager(IAuthorizationConfiguration authorizationConfiguration, ILogger<AuthorizationManager> logger) : base(authorizationConfiguration, logger) {
        var isVisibleDict = new Dictionary<Type, Func<object, object, string, IContext, bool>> {
            { defaultAuthorizer, FactoryUtils.CreateFunctionalTypeAuthorizerDelegate(defaultAuthorizer.GetMethod("IsVisible")) }
        };

        if (typeAuthorizers.Any()) {
            if (typeAuthorizers.Values.Any(t => typeof(ITypeAuthorizer<object>).IsAssignableFrom(t))) {
                throw new InitialisationException(logger.LogAndReturn("Only Default Authorizer can be ITypeAuthorizer<object>"));
            }

            isVisibleDelegates = isVisibleDict.Union(typeAuthorizers.Values.ToDictionary(type => type, type => FactoryUtils.CreateFunctionalTypeAuthorizerDelegate(type.GetMethod("IsVisible")))).ToImmutableDictionary();
        }
        else {
            // default authorizer must be the only TypeAuthorizer
            isVisibleDelegates = isVisibleDict.ToImmutableDictionary();
        }
    }

    public override IFacet Decorate(IFacet facet, ISpecification holder) {
        var facetType = facet.FacetType;
        var identifier = holder.Identifier;

        if (facetType == typeof(IHideForSessionFacet)) {
            return new AuthorizationHideForSessionFacet(identifier, this);
        }

        return facet;
    }

    protected override object CreateAuthorizer(Type type, ILifecycleManager lifecycleManager) => lifecycleManager.CreateNonAdaptedObject(type);
    private static FunctionalContext FunctionalContext(INakedFramework framework) => new() { Persistor = framework.Persistor, Provider = framework.ServiceProvider };

    private bool IsMenuVisible(INakedFramework framework, IIdentifier identifier) {
        var authorizerType = typeAuthorizers.Where(ta => ta.Key == typeof(string).FullName).Select(ta => ta.Value).FirstOrDefault();
        if (authorizerType is not null) {
            if (CreateAuthorizer(authorizerType, framework.LifecycleManager) is IMainMenuAuthorizer menuAuth) {
                return menuAuth.IsVisible(identifier.ClassName, identifier.MemberName, FunctionalContext(framework));
            }
        }

        return true;
    }

    private bool IsObjectVisible(INakedFramework framework, INakedObjectAdapter target, IIdentifier identifier) {
        var authorizer = GetAuthorizer(target, framework.LifecycleManager);

        if (authorizer is INamespaceAuthorizer nameAuth) {
            return nameAuth.IsVisible(target.Object, identifier.MemberName, FunctionalContext(framework));
        }

        //Must be an ITypeAuthorizer, including default authorizer (ITypeAuthorizer<object>)
        return isVisibleDelegates[authorizer.GetType()](authorizer, target.GetDomainObject(), identifier.MemberName, FunctionalContext(framework));
    }

    public override bool IsVisible(INakedFramework framework, INakedObjectAdapter target, IIdentifier identifier) =>
        target switch {
            null => IsMenuVisible(framework, identifier),
            _ => IsObjectVisible(framework, target, identifier)
        };

    public override bool IsEditable(INakedFramework framework, INakedObjectAdapter target, IIdentifier identifier) => false;
}