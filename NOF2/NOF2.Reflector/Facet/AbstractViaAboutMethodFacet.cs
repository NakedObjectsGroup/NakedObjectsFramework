﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NOF2.About;
using NOF2.Reflector.Component;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public abstract class AbstractViaAboutMethodFacet : FacetAbstract, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    protected AbstractViaAboutMethodFacet(MethodInfo method, AboutHelpers.AboutType aboutType, ILogger logger) {
        methodWrapper = SerializationFactory.Wrap(method, logger);
        AboutType = aboutType;
    }

    private AboutHelpers.AboutType AboutType { get; }

    private static IAboutCache GetCache(INakedFramework framework) => framework.ServiceProvider.GetService<IAboutCache>() ?? throw new InvalidOperationException("Cannot find about cache");

    protected IAbout InvokeAboutMethod(INakedFramework framework, object target, AboutTypeCodes typeCode, bool substitute, bool flagNull, params object[] proposedValues) {
        if (target is null && !GetMethod().IsStatic) {
            if (flagNull) {
                throw new InvalidOperationException("Unexpected null object on instance about method");
            }

            return null;
        }

        IAbout About() {
            var about = AboutType.AboutFactory(typeCode, framework);
            methodWrapper.Invoke(target, GetMethod().GetParameters(framework, about, substitute, proposedValues));
            return about;
        }

        return GetCache(framework).GetOrCacheAbout(target, GetMethod(), typeCode, About);
    }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}