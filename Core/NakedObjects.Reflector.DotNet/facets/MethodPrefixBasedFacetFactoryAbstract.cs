// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Disable;
using NakedObjects.Architecture.Facets.Hide;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Actions.Executed;
using NakedObjects.Reflector.DotNet.Facets.Disable;
using NakedObjects.Reflector.DotNet.Facets.Hide;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;

namespace NakedObjects.Reflector.DotNet.Facets {
    public abstract class MethodPrefixBasedFacetFactoryAbstract : FacetFactoryAbstract, IMethodPrefixBasedFacetFactory {
        protected MethodPrefixBasedFacetFactoryAbstract(INakedObjectReflector reflector, NakedObjectFeatureType[] featureTypes)
            : base(reflector, featureTypes) {}

        #region IMethodPrefixBasedFacetFactory Members

        public abstract string[] Prefixes { get; }

        #endregion

        protected MethodInfo FindMethodWithOrWithoutParameters(Type type, MethodType methodType, string name, Type returnType, Type[] parms) {
            return FindMethod(type, methodType, name, returnType, parms) ??
                   FindMethod(type, methodType, name, returnType, Type.EmptyTypes);
        }

        /// <summary>
        ///     Returns  specific public methods that: have the specified prefix; have the specified return Type, or
        ///     void, and has the specified number of parameters. If the returnType is specified as null then the return
        ///     Type is ignored.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodType"></param>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        protected MethodInfo[] FindMethods(Type type,
                                           MethodType methodType,
                                           string name,
                                           Type returnType) {
            return type.GetMethods(GetBindingFlagsForMethodType(methodType)).
                        Where(m => m.Name == name).
                        Where(m => (m.IsStatic && methodType == MethodType.Class) || (!m.IsStatic && methodType == MethodType.Object)).
                        Where(m => AttributeUtils.GetCustomAttribute<NakedObjectsIgnoreAttribute>(m) == null).
                        Where(m => returnType == null || returnType.IsAssignableFrom(m.ReturnType)).ToArray();
        }


        /// <summary>
        ///     Returns  specific public methods that: have the specified prefix; have the specified return Type, or
        ///     void, and has the specified number of parameters. If the returnType is specified as null then the return
        ///     Type is ignored.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodType"></param>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="paramTypes">the set of parameters the method should have, if null then is ignored</param>
        /// <param name="paramNames">the names of the parameters the method should have, if null then is ignored</param>
        protected MethodInfo FindMethod(Type type,
                                        MethodType methodType,
                                        string name,
                                        Type returnType,
                                        Type[] paramTypes,
                                        string[] paramNames = null) {
            try {
                MethodInfo method = paramTypes == null
                                        ? type.GetMethod(name, GetBindingFlagsForMethodType(methodType))
                                        : type.GetMethod(name, GetBindingFlagsForMethodType(methodType), null, paramTypes, null);

                if (method == null) {
                    return null;
                }

                // check for static modifier
                if (method.IsStatic && methodType == MethodType.Object) {
                    return null;
                }

                if (!method.IsStatic && methodType == MethodType.Class) {
                    return null;
                }

                if (AttributeUtils.GetCustomAttribute<NakedObjectsIgnoreAttribute>(method) != null) {
                    return null;
                }

                // check for return Type
                if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType)) {
                    return null;
                }

                if (paramNames != null) {
                    string[] methodParamNames = method.GetParameters().Select(p => p.Name).ToArray();

                    if (!paramNames.SequenceEqual(methodParamNames)) {
                        return null;
                    }
                }

                return method;
            }
            catch (AmbiguousMatchException e) {
                throw new ModelException(string.Format(Resources.NakedObjects.AmbiguousMethodError, name, type.FullName), e);
            }
        }

        private BindingFlags GetBindingFlagsForMethodType(MethodType methodType) {
            return BindingFlags.Public |
                   (methodType == MethodType.Object ? BindingFlags.Instance : BindingFlags.Static) |
                   (Reflector.IgnoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default);
        }

        protected static void RemoveMethod(IMethodRemover methodRemover, MethodInfo method) {
            if (methodRemover != null && method != null) {
                methodRemover.RemoveMethod(method);
            }
        }

        protected static Type[] ParamTypesOrNull(Type type) {
            return type == null ? Type.EmptyTypes : new[] {type};
        }

        protected void FindAndRemoveDisableMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, IFacetHolder facetHolder) {
            FindAndRemoveDisableMethod(facets, methodRemover, type, methodType, capitalizedName, (Type) null, facetHolder);
        }

        protected void FindAndRemoveDisableMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type paramType, IFacetHolder facetHolder) {
            FindAndRemoveDisableMethod(facets, methodRemover, type, methodType, capitalizedName, ParamTypesOrNull(paramType), facetHolder);
        }

        protected void FindDefaultDisableMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] paramTypes, IFacetHolder facetHolder) {
            MethodInfo method = FindMethodWithOrWithoutParameters(type, methodType, PrefixesAndRecognisedMethods.DisablePrefix + capitalizedName, typeof (string), paramTypes);
            if (method != null) {
                facets.Add(new DisableForContextFacetViaMethod(method, facetHolder));
            }
        }

        protected void FindAndRemoveDisableMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] paramTypes, IFacetHolder facetHolder) {
            MethodInfo method = FindMethodWithOrWithoutParameters(type, methodType, PrefixesAndRecognisedMethods.DisablePrefix + capitalizedName, typeof (string), paramTypes);
            if (method != null) {
                methodRemover.RemoveMethod(method);
                facets.Add(new DisableForContextFacetViaMethod(method, facetHolder));
            }
        }

        protected void FindAndRemoveHideMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, IFacetHolder facetHolder) {
            FindAndRemoveHideMethod(facets, methodRemover, type, methodType, capitalizedName, (Type) null, facetHolder);
        }

        protected void FindAndRemoveHideMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type collectionType, IFacetHolder facetHolder) {
            FindAndRemoveHideMethod(facets, methodRemover, type, methodType, capitalizedName, ParamTypesOrNull(collectionType), facetHolder);
        }

        protected void FindDefaultHideMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] paramTypes, IFacetHolder facetHolder) {
            MethodInfo method = FindMethodWithOrWithoutParameters(type, methodType, PrefixesAndRecognisedMethods.HidePrefix + capitalizedName, typeof (bool), paramTypes);
            if (method != null) {
                facets.Add(new HideForContextFacetViaMethod(method, facetHolder));
                AddOrAddToExecutedWhereFacet(method, facetHolder);
            }
        }

        protected void FindAndRemoveHideMethod(IList<IFacet> facets, IMethodRemover methodRemover, Type type, MethodType methodType, string capitalizedName, Type[] paramTypes, IFacetHolder facetHolder) {
            MethodInfo method = FindMethodWithOrWithoutParameters(type, methodType, PrefixesAndRecognisedMethods.HidePrefix + capitalizedName, typeof (bool), paramTypes);
            if (method != null) {
                methodRemover.RemoveMethod(method);
                facets.Add(new HideForContextFacetViaMethod(method, facetHolder));
                AddOrAddToExecutedWhereFacet(method, facetHolder);
            }
        }

        protected static void AddHideForSessionFacetNone(IList<IFacet> facets, IFacetHolder facetHolder) {
            facets.Add(new HideForSessionFacetNone(facetHolder));
        }

        protected static void AddDisableForSessionFacetNone(IList<IFacet> facets, IFacetHolder facetHolder) {
            facets.Add(new DisableForSessionFacetNone(facetHolder));
        }

        protected static void AddDisableFacetAlways(IList<IFacet> facets, IFacetHolder facetHolder) {
            facets.Add(new DisabledFacetAlways(facetHolder));
        }

        protected static void AddOrAddToExecutedWhereFacet(MethodInfo method, IFacetHolder holder) {
            var attribute = AttributeUtils.GetCustomAttribute<ExecutedAttribute>(method);
            if (attribute != null && !attribute.IsAjax) {
                var executedFacet = holder.GetFacet<IExecutedControlMethodFacet>();
                if (executedFacet == null) {
                    FacetUtils.AddFacet(new ExecutedFacetAnnotationForControlMethods(method, attribute.Value, holder));
                }
                else {
                    executedFacet.AddMethodExecutedWhere(method, attribute.Value);
                }
            }
        }

        protected static void AddAjaxFacet(MethodInfo method, IFacetHolder holder) {
            if (method == null) {
                FacetUtils.AddFacet(new AjaxFacetAnnotation(holder));
            }
            else {
                var attribute = AttributeUtils.GetCustomAttribute<ExecutedAttribute>(method);
                if (attribute != null && attribute.IsAjax) {
                    if (attribute.AjaxValue == Ajax.Disabled) {
                        FacetUtils.AddFacet(new AjaxFacetAnnotation(holder));
                    }
                }
            }
        }
    }
}