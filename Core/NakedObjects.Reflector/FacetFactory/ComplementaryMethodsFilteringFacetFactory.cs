// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public class ComplementaryMethodsFilteringFacetFactory : FacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UnsupportedParameterTypesMethodFilteringFactory));

        /// <summary>
        ///     The <see cref="IFacet" />s registered are the generic ones from no-architecture (where they exist)
        /// </summary>
        public ComplementaryMethodsFilteringFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ActionsOnly) {}

        #region IMethodFilteringFacetFactory Members

        public bool Filters(MethodInfo method) {
            return IsComplementaryMethod(method);
        }

        #endregion

        private static bool IsComplementaryMethod(MethodInfo actionMethod) {
            var propertyPrefixes = new[] {
                PrefixesAndRecognisedMethods.AutoCompletePrefix,
                PrefixesAndRecognisedMethods.ModifyPrefix,
                PrefixesAndRecognisedMethods.ClearPrefix,
                PrefixesAndRecognisedMethods.ChoicesPrefix,
                PrefixesAndRecognisedMethods.DefaultPrefix,
                PrefixesAndRecognisedMethods.ValidatePrefix,
                PrefixesAndRecognisedMethods.HidePrefix,
                PrefixesAndRecognisedMethods.DisablePrefix
            };

            var actionPrefixes = new[] {
                PrefixesAndRecognisedMethods.ValidatePrefix,
                PrefixesAndRecognisedMethods.HidePrefix,
                PrefixesAndRecognisedMethods.DisablePrefix
            };

            var parameterPrefixes = new[] {
                PrefixesAndRecognisedMethods.AutoCompletePrefix,
                PrefixesAndRecognisedMethods.ParameterChoicesPrefix,
                PrefixesAndRecognisedMethods.ParameterDefaultPrefix,
                PrefixesAndRecognisedMethods.ValidatePrefix
            };


            return propertyPrefixes.Any(prefix => IsComplementaryPropertyMethod(actionMethod, prefix)) ||
                   actionPrefixes.Any(prefix => IsComplementaryActionMethod(actionMethod, prefix)) ||
                   parameterPrefixes.Any(prefix => IsComplementaryParameterMethod(actionMethod, prefix));
        }

        private static bool IsComplementaryPropertyMethod(MethodInfo actionMethod, string prefix) {
            string propertyName;
            if (MatchesPrefix(actionMethod, prefix, out propertyName)) {
                if (InheritsProperty(actionMethod.DeclaringType.BaseType, propertyName)) {
                    Log.InfoFormat("Filtering method {0} because of property {1} on {2}", actionMethod.Name, propertyName, actionMethod.DeclaringType.BaseType.FullName);
                    return true;
                }
            }
            return false;
        }

        private static bool IsComplementaryActionMethod(MethodInfo actionMethod, string prefix) {
            string propertyName;
            if (MatchesPrefix(actionMethod, prefix, out propertyName)) {
                if (InheritsMethod(actionMethod.DeclaringType.BaseType, propertyName)) {
                    Log.InfoFormat("Filtering method {0} because of action {1} on {2}", actionMethod.Name, propertyName, actionMethod.DeclaringType.BaseType.FullName);
                    return true;
                }
            }
            return false;
        }

        private static bool IsComplementaryParameterMethod(MethodInfo actionMethod, string prefix) {
            string propertyName;
            if (MatchesPrefix(actionMethod, prefix, out propertyName)) {
                propertyName = TrimDigits(propertyName);
                if (InheritsMethod(actionMethod.DeclaringType.BaseType, propertyName)) {
                    Log.InfoFormat("Filtering method {0} because of action {1} on {2}", actionMethod.Name, propertyName, actionMethod.DeclaringType.BaseType.FullName);
                    return true;
                }
            }
            return false;
        }

        private static string TrimDigits(string toTrim) {
            while (toTrim.Length > 0 && char.IsDigit(toTrim, 0)) {
                toTrim = toTrim.Substring(1);
            }

            return toTrim;
        }


        private static bool MatchesPrefix(MethodInfo actionMethod, string prefix, out string propertyName) {
            if (actionMethod.Name.StartsWith(prefix)) {
                propertyName = actionMethod.Name.Remove(0, prefix.Length);
                return true;
            }
            propertyName = null;
            return false;
        }

        private static bool InheritsProperty(Type typeToCheck, string name) {
            if (typeToCheck == null) {
                return false;
            }

            if (typeToCheck.GetProperty(name) != null) {
                return true;
            }

            return InheritsProperty(typeToCheck.BaseType, name);
        }

        private static bool InheritsMethod(Type typeToCheck, string name) {
            if (typeToCheck == null) {
                return false;
            }

            if (typeToCheck.GetMethod(name) != null) {
                return true;
            }

            return InheritsMethod(typeToCheck.BaseType, name);
        }
    }
}