using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflector.FacetFactory {
    public static class MethodHelpers {
        public static MethodInfo FindMethodWithOrWithoutParameters(IReflector reflector, Type type, MethodType methodType, string name, Type returnType, Type[] parms) =>
            FindMethod(reflector, type, methodType, name, returnType, parms) ??
            FindMethod(reflector, type, methodType, name, returnType, Type.EmptyTypes);

        /// <summary>
        ///     Returns  specific public methods that: have the specified prefix; have the specified return Type, or
        ///     void, and has the specified number of parameters. If the returnType is specified as null then the return
        ///     Type is ignored.
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="type"></param>
        /// <param name="methodType"></param>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        public static MethodInfo[] FindMethods(IReflector reflector,
                                               Type type,
                                               MethodType methodType,
                                               string name,
                                               Type returnType = null) =>
            type.GetMethods(GetBindingFlagsForMethodType(methodType, reflector)).Where(m => m.Name == name).Where(m => m.IsStatic && methodType == MethodType.Class || !m.IsStatic && methodType == MethodType.Object).Where(m => m.GetCustomAttribute<NakedObjectsIgnoreAttribute>() == null).Where(m => returnType == null || returnType.IsAssignableFrom(m.ReturnType)).ToArray();

        /// <summary>
        ///     Returns  specific public methods that: have the specified prefix; have the specified return Type, or
        ///     void, and has the specified number of parameters. If the returnType is specified as null then the return
        ///     Type is ignored.
        /// </summary>
        /// <param name="reflector"></param>
        /// <param name="type"></param>
        /// <param name="methodType"></param>
        /// <param name="name"></param>
        /// <param name="returnType"></param>
        /// <param name="paramTypes">the set of parameters the method should have, if null then is ignored</param>
        /// <param name="paramNames">the names of the parameters the method should have, if null then is ignored</param>
        public static MethodInfo FindMethod(IReflector reflector,
                                            Type type,
                                            MethodType methodType,
                                            string name,
                                            Type returnType,
                                            Type[] paramTypes,
                                            string[] paramNames = null) {
            try {
                var method = paramTypes == null
                    ? type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector))
                    : type.GetMethod(name, GetBindingFlagsForMethodType(methodType, reflector), null, paramTypes, null);

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

                if (reflector.ClassStrategy.IsIgnored(method)) {
                    return null;
                }

                // check for return Type
                if (returnType != null && !returnType.IsAssignableFrom(method.ReturnType)) {
                    return null;
                }

                if (paramNames != null) {
                    var methodParamNames = method.GetParameters().Select(p => p.Name).ToArray();

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

        public static BindingFlags GetBindingFlagsForMethodType(MethodType methodType, IReflector reflector) =>
            BindingFlags.Public |
            (methodType == MethodType.Object ? BindingFlags.Instance : BindingFlags.Static) |
            (reflector.IgnoreCase ? BindingFlags.IgnoreCase : BindingFlags.Default);

        public static void RemoveMethod(IMethodRemover methodRemover, MethodInfo method) {
            if (method != null) {
                methodRemover?.RemoveMethod(method);
            }
        }

        public static void FindDefaultDisableMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory ) {
            var method = FindMethodWithOrWithoutParameters(reflector, type, methodType, RecognisedMethodsAndPrefixes.DisablePrefix + capitalizedName, typeof(string), Type.EmptyTypes);
            if (method != null) {
                facets.Add(new DisableForContextFacet(method, specification, loggerFactory.CreateLogger<DisableForContextFacet>()));
            }
        }

        public static void FindAndRemoveDisableMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory, IMethodRemover methodRemover = null) {
            var method = FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.DisablePrefix + capitalizedName, typeof(string), Type.EmptyTypes);
            if (method != null) {
                methodRemover?.RemoveMethod(method);
                facets.Add(new DisableForContextFacet(method, specification, loggerFactory.CreateLogger<DisableForContextFacet>()));
            }
        }

        public static void FindDefaultHideMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory) {
            var method = FindMethodWithOrWithoutParameters(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                facets.Add(new HideForContextFacet(method, specification, loggerFactory.CreateLogger<HideForContextFacet>()));
                AddOrAddToExecutedWhereFacet(method, specification);
            }
        }

        public static void FindAndRemoveHideMethod(IReflector reflector, IList<IFacet> facets, Type type, MethodType methodType, string capitalizedName, ISpecification specification, ILoggerFactory loggerFactory, IMethodRemover methodRemover = null) {
            var method = FindMethod(reflector, type, methodType, RecognisedMethodsAndPrefixes.HidePrefix + capitalizedName, typeof(bool), Type.EmptyTypes);
            if (method != null) {
                methodRemover?.RemoveMethod(method);
                facets.Add(new HideForContextFacet(method, specification, loggerFactory.CreateLogger<HideForContextFacet>()));
                AddOrAddToExecutedWhereFacet(method, specification);
            }
        }

        public static void AddHideForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(new HideForSessionFacetNone(specification));

        public static void AddDisableForSessionFacetNone(IList<IFacet> facets, ISpecification specification) => facets.Add(new DisableForSessionFacetNone(specification));

        public static void AddDisableFacetAlways(IList<IFacet> facets, ISpecification specification) => facets.Add(new DisabledFacetAlways(specification));

        public static void AddOrAddToExecutedWhereFacet(MethodInfo method, ISpecification holder) {
            var attribute = method.GetCustomAttribute<ExecutedAttribute>();
            if (attribute != null && !attribute.IsAjax) {
                var executedFacet = holder.GetFacet<IExecutedControlMethodFacet>();
                if (executedFacet == null) {
                    FacetUtils.AddFacet(new ExecutedControlMethodFacet(method, attribute.Value, holder));
                }
                else {
                    executedFacet.AddMethodExecutedWhere(method, attribute.Value);
                }
            }
        }

        public static void AddAjaxFacet(MethodInfo method, ISpecification holder) {
            if (method == null) {
                FacetUtils.AddFacet(new AjaxFacet(holder));
            }
            else {
                var attribute = method.GetCustomAttribute<ExecutedAttribute>();
                if (attribute != null && attribute.IsAjax) {
                    if (attribute.AjaxValue == Ajax.Disabled) {
                        FacetUtils.AddFacet(new AjaxFacet(holder));
                    }
                }
            }
        }
    }
}