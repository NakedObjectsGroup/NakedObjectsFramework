using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
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
}