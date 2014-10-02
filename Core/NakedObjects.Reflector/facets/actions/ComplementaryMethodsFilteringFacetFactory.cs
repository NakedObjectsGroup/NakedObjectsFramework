// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.DotNet.Facets.Actions {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public class ComplementaryMethodsFilteringFacetFactory : FacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UnsupportedParameterTypesMethodFilteringFactory));

        /// <summary>
        ///     The <see cref="IFacet" />s registered are the generic ones from no-architecture (where they exist)
        /// </summary>
        public ComplementaryMethodsFilteringFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.ActionsOnly) {}

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