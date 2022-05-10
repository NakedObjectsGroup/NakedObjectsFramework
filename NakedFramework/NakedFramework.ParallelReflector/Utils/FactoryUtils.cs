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
}