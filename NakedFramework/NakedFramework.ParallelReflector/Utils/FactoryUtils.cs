using System;
using System.Collections.Generic;
using System.Reflection;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.ParallelReflector.Utils;

public static class FactoryUtils {
    public static bool IsVoid(Type type) => type == typeof(void);

    public static bool IsSealed(Type type) => type.IsSealed;

    public static bool IsInterface(Type type) => type.IsInterface;

    public static bool IsAbstract(Type type) => type.IsAbstract;

    public static bool IsStatic(Type type) => IsAbstract(type) && IsSealed(type);

    public static void AddTypeFacets(ISpecificationBuilder specification, Type type) {
        var facets = new List<IFacet> {
            new TypeFacet(type)
        };

        if (IsAbstract(type)) {
            facets.Add(TypeIsAbstractFacet.Instance);
        }

        if (IsInterface(type)) {
            facets.Add(TypeIsInterfaceFacet.Instance);
        }

        if (IsSealed(type)) {
            facets.Add(TypeIsSealedFacet.Instance);
        }

        if (IsVoid(type)) {
            facets.Add(TypeIsVoidFacet.Instance);
        }

        if (IsStatic(type)) {
            facets.Add(TypeIsStaticFacet.Instance);
        }

        FacetUtils.AddFacets(facets, specification);
    }

    public static T Invoke<T>(this Func<object, object[], object> methodDelegate, MethodInfo method, object target, object[] parms) {
        try {
            return methodDelegate is not null ? (T)methodDelegate(target, parms) : (T)method.Invoke(target, parms);
        }
        catch (InvalidCastException e) {
            throw new NakedObjectDomainException($"Cast error on method: {method} : {e.Message}");
        }
    }

    public static T Invoke<T>(this Func<object, object[], object> methodDelegate, MethodInfo method, object[] parms) {
        try {
            return methodDelegate is not null ? (T)methodDelegate(null, parms) : (T)method.Invoke(null, parms);
        }
        catch (InvalidCastException e) {
            throw new NakedObjectDomainException($"Cast error on method: {method} : {e.Message}");
        }
    }

    public static void Invoke(this Func<object, object[], object> methodDelegate, MethodInfo method, object target, object[] parms) {
        if (methodDelegate is not null) {
            methodDelegate(target, parms);
        }
        else {
            method.Invoke(target, parms);
        }
    }
}