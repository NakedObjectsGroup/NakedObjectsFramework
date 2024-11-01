// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Core.Error;

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Sets up all the <see cref="IFacet" />s for an action in a single shot
/// </summary>
public sealed class ComplementaryMethodsFilteringFacetFactory : DomainObjectFacetFactoryProcessor, IMethodFilteringFacetFactory {
    private static readonly string[] PropertyPrefixes = {
        RecognisedMethodsAndPrefixes.AutoCompletePrefix,
        RecognisedMethodsAndPrefixes.ModifyPrefix,
        RecognisedMethodsAndPrefixes.ChoicesPrefix,
        RecognisedMethodsAndPrefixes.DefaultPrefix,
        RecognisedMethodsAndPrefixes.ValidatePrefix,
        RecognisedMethodsAndPrefixes.HidePrefix,
        RecognisedMethodsAndPrefixes.DisablePrefix
    };

    private static readonly string[] ActionPrefixes = {
        RecognisedMethodsAndPrefixes.ValidatePrefix,
        RecognisedMethodsAndPrefixes.HidePrefix,
        RecognisedMethodsAndPrefixes.DisablePrefix
    };

    private static readonly string[] ParameterPrefixes = {
        RecognisedMethodsAndPrefixes.AutoCompletePrefix,
        RecognisedMethodsAndPrefixes.ParameterChoicesPrefix,
        RecognisedMethodsAndPrefixes.ParameterDefaultPrefix,
        RecognisedMethodsAndPrefixes.ValidatePrefix
    };

    private readonly ILogger<ComplementaryMethodsFilteringFacetFactory> logger;

    /// <summary>
    ///     The <see cref="IFacet" />s registered are the generic ones from no-architecture (where they exist)
    /// </summary>
    /// <param name="numericOrder"></param>
    /// <param name="loggerFactory"></param>
    public ComplementaryMethodsFilteringFacetFactory(IFacetFactoryOrder<ComplementaryMethodsFilteringFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<ComplementaryMethodsFilteringFacetFactory>();

    #region IMethodFilteringFacetFactory Members

    public bool Filters(MethodInfo method, IClassStrategy classStrategy) => IsComplementaryMethod(method);

    #endregion

    private bool IsComplementaryMethod(MethodInfo actionMethod) =>
        PropertyPrefixes.Any(prefix => IsComplementaryPropertyMethod(actionMethod, prefix)) ||
        ActionPrefixes.Any(prefix => IsComplementaryActionMethod(actionMethod, prefix)) ||
        ParameterPrefixes.Any(prefix => IsComplementaryParameterMethod(actionMethod, prefix));

    private static bool IsNotOverride(MethodInfo method) => method.GetBaseDefinition().DeclaringType == method.DeclaringType;

    private bool IsComplementaryPropertyMethod(MethodInfo actionMethod, string prefix) {
        if (MatchesPrefix(actionMethod, prefix, out var propertyName)) {
            var declaringType = actionMethod.DeclaringType;
            if (declaringType == null) {
                throw new NakedObjectSystemException("declaring type cannot be null");
            }

            var baseType = declaringType.BaseType;

            if (InheritsProperty(baseType, propertyName)) {
                if (IsNotOverride(actionMethod)) {
                    // no override so flag
                    logger.LogWarning($"Filtering method {actionMethod.Name} because of property {propertyName} on {baseType?.FullName}");
                }

                return true;
            }
        }

        return false;
    }

    private bool IsComplementaryActionMethod(MethodInfo actionMethod, string prefix) {
        if (MatchesPrefix(actionMethod, prefix, out var propertyName)) {
            var declaringType = actionMethod.DeclaringType;
            if (declaringType == null) {
                throw new NakedObjectSystemException("declaring type cannot be null");
            }

            if (InheritsMethod(declaringType.BaseType, propertyName)) {
                if (IsNotOverride(actionMethod)) {
                    // no override so flag
                    var baseTypeName = declaringType.BaseType == null ? "Unknown type" : declaringType.BaseType.FullName;
                    logger.LogWarning($"Filtering method {actionMethod.Name} because of action {propertyName} on {baseTypeName}");
                }

                return true;
            }
        }

        return false;
    }

    private bool IsComplementaryParameterMethod(MethodInfo actionMethod, string prefix) {
        if (MatchesPrefix(actionMethod, prefix, out var propertyName)) {
            propertyName = TrimDigits(propertyName);
            var declaringType = actionMethod.DeclaringType;
            if (declaringType == null) {
                throw new NakedObjectSystemException("declaring type cannot be null");
            }

            if (InheritsMethod(declaringType.BaseType, propertyName)) {
                if (IsNotOverride(actionMethod)) {
                    // no override so flag
                    var baseTypeName = declaringType.BaseType == null ? "Unknown type" : declaringType.BaseType.FullName;
                    logger.LogWarning($"Filtering method {actionMethod.Name} because of action {propertyName} on {baseTypeName}");
                }

                return true;
            }
        }

        return false;
    }

    private static string TrimDigits(string toTrim) {
        while (toTrim.Length > 0 && char.IsDigit(toTrim, 0)) {
            toTrim = toTrim[1..];
        }

        return toTrim;
    }

    private static bool MatchesPrefix(MethodInfo actionMethod, string prefix, out string propertyName) {
        if (actionMethod.Name.StartsWith(prefix, StringComparison.Ordinal)) {
            propertyName = actionMethod.Name.Remove(0, prefix.Length);
            return true;
        }

        propertyName = null;
        return false;
    }

    private static bool InheritsProperty(Type typeToCheck, string name) => typeToCheck != null && (typeToCheck.GetProperty(name) != null || InheritsProperty(typeToCheck.BaseType, name));

    private static bool InheritsMethod(Type typeToCheck, string name) => typeToCheck != null && (typeToCheck.GetMethods().Any(m => m.Name.Equals(name, StringComparison.Ordinal)) || InheritsMethod(typeToCheck.BaseType, name));
}