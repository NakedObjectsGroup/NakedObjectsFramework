// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Facets.AutoComplete;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.Access;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted;
using NakedObjects.Reflector.DotNet.Facets.Properties.Choices;
using NakedObjects.Reflector.DotNet.Facets.Properties.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Properties.Modify;
using NakedObjects.Reflector.DotNet.Facets.Properties.Validate;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;
using NakedObjects.Value;

namespace NakedObjects.Reflector.DotNet.Facets.Properties {
    public class PropertyMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {
        private static readonly ILog Log;
        private static readonly string[] prefixes;

        static PropertyMethodsFacetFactory() {
            Log = LogManager.GetLogger(typeof (PropertyMethodsFacetFactory));

            prefixes = new[] {
                PrefixesAndRecognisedMethods.ClearPrefix,
                PrefixesAndRecognisedMethods.ModifyPrefix
            };
        }

        public PropertyMethodsFacetFactory()
            : base(NakedObjectFeatureType.PropertiesOnly) {}

        public PropertyMethodsFacetFactory(string[] subPefixes)
            : base(NakedObjectFeatureType.PropertiesOnly) {}

        public override string[] Prefixes {
            get { return prefixes; }
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            string capitalizedName = property.Name;
            var paramTypes = new[] {property.PropertyType};

            var facets = new List<IFacet> {new PropertyAccessorFacetViaAccessor(property, holder)};

            if (property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))) {
                facets.Add(new NullableFacetAlways(holder));
            }

            if (property.GetSetMethod() != null) {
                if (property.PropertyType == typeof (byte[])) {
                    facets.Add(new DisabledFacetAlways(holder));
                }
                else {
                    facets.Add(new PropertySetterFacetViaSetterMethod(property, holder));
                }
                facets.Add(new PropertyInitializationFacetViaSetterMethod(property, holder));
            }
            else {
                //facets.Add(new DerivedFacetInferred(holder));
                facets.Add(new NotPersistedFacetAnnotation(holder));
                facets.Add(new DisabledFacetAlways(holder));
            }
            FindAndRemoveModifyMethod(facets, methodRemover, property.DeclaringType, capitalizedName, paramTypes, holder);
            FindAndRemoveClearMethod(facets, methodRemover, property.DeclaringType, capitalizedName, holder);

            FindAndRemoveAutoCompleteMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, holder);
            FindAndRemoveChoicesMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, holder);
            FindAndRemoveDefaultMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, holder);
            FindAndRemoveValidateMethod(facets, methodRemover, property.DeclaringType, paramTypes, capitalizedName, holder);

            AddHideForSessionFacetNone(facets, holder);
            AddDisableForSessionFacetNone(facets, holder);
            FindDefaultHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], holder);
            FindAndRemoveHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, property.PropertyType, holder);
            FindDefaultDisableMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], holder);
            FindAndRemoveDisableMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, property.PropertyType, holder);

            return FacetUtils.AddFacets(facets);
        }

        private void FindAndRemoveModifyMethod(ICollection<IFacet> propertyFacets,
                                               IMethodRemover methodRemover,
                                               Type type,
                                               string capitalizedName,
                                               Type[] parms,
                                               IFacetHolder property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ModifyPrefix + capitalizedName, typeof (void), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertySetterFacetViaModifyMethod(method, property));
            }
        }

        private void FindAndRemoveClearMethod(ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, string capitalizedName, IFacetHolder property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ClearPrefix + capitalizedName, typeof (void), Type.EmptyTypes);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                Log.WarnFormat(@"'Clear' method '{0}' has been found on '{1}' : The 'Clear' method is considered obsolete, use 'Modify' instead", PrefixesAndRecognisedMethods.ClearPrefix + capitalizedName, type.FullName);
                propertyFacets.Add(new PropertyClearFacetViaClearMethod(method, property));
            }
        }


        private void FindAndRemoveValidateMethod(ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, Type[] parms, string capitalizedName, IFacetHolder property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ValidatePrefix + capitalizedName, typeof (string), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyValidateFacetViaMethod(method, property));
                AddAjaxFacet(method, property);
            }
            else {
                AddAjaxFacet(null, property);
            }
        }

        private void FindAndRemoveDefaultMethod(ICollection<IFacet> propertyFacets,
                                                IMethodRemover methodRemover,
                                                Type type,
                                                string capitalizedName,
                                                Type returnType,
                                                IFacetHolder property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.DefaultPrefix + capitalizedName, returnType, Type.EmptyTypes);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyDefaultFacetViaMethod(method, property));
                AddOrAddToExecutedWhereFacet(method, property);
            }
        }

        private void FindAndRemoveChoicesMethod(ICollection<IFacet> propertyFacets,
                                                IMethodRemover methodRemover,
                                                Type type,
                                                string capitalizedName,
                                                Type returnType,
                                                IFacetHolder property) {
            MethodInfo[] methods = FindMethods(type,
                                               MethodType.Object,
                                               PrefixesAndRecognisedMethods.ChoicesPrefix + capitalizedName,
                                               typeof (IEnumerable<>).MakeGenericType(returnType));

            if (methods.Length > 1) {
                methods.Skip(1).ForEach(m => Log.WarnFormat("Found multiple choices methods: {0} in type: {1} ignoring method(s) with params: {2}",
                                                            PrefixesAndRecognisedMethods.ChoicesPrefix + capitalizedName,
                                                            type,
                                                            m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)));
            }

            MethodInfo method = methods.FirstOrDefault();
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyChoicesFacetViaMethod(method, property));
                AddOrAddToExecutedWhereFacet(method, property);
            }
        }

        private void FindAndRemoveAutoCompleteMethod(ICollection<IFacet> propertyFacets,
                                                     IMethodRemover methodRemover,
                                                     Type type,
                                                     string capitalizedName,
                                                     Type returnType,
                                                     IFacetHolder property) {
            // only support if property is string or domain type
            if (returnType.IsClass || returnType.IsInterface) {
                MethodInfo method = FindMethod(type,
                                               MethodType.Object,
                                               PrefixesAndRecognisedMethods.AutoCompletePrefix + capitalizedName,
                                               typeof (IQueryable<>).MakeGenericType(returnType),
                                               new[] {typeof (string)});

                if (method != null) {
                    var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                    var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof (MinLengthAttribute));

                    int pageSize = pageSizeAttr != null ? pageSizeAttr.Value : 0; // default to 0 ie system default
                    int minLength = minLengthAttr != null ? minLengthAttr.Length : 0;

                    RemoveMethod(methodRemover, method);
                    propertyFacets.Add(new AutoCompleteFacetViaMethod(method, pageSize, minLength, property));
                    AddOrAddToExecutedWhereFacet(method, property);
                }
            }
        }


        public override void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            foreach (PropertyInfo property in candidates) {
                if (property.GetGetMethod() != null &&
                    property.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null &&
                    !CollectionUtils.IsQueryable(property.PropertyType)) {
                    methodListToAppendTo.Add(property);
                }
            }
            candidates.Clear();
        }
    }
}