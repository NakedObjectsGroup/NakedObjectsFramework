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

    public static void AddTypeFacets(ISpecification specification, Type type) {
        var facets = new List<IFacet> {
            new TypeIsAbstractFacet(specification, IsAbstract(type)),
            new TypeIsInterfaceFacet(specification, IsInterface(type)),
            new TypeIsSealedFacet(specification, IsSealed(type)),
            new TypeIsVoidFacet(specification, IsVoid(type)),
            new TypeIsStaticFacet(specification, IsStatic(type)),
            new TypeFacet(specification, type)
        };

        FacetUtils.AddFacets(facets);
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