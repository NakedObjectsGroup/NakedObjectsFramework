using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Component;
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

    private static IEnumerable<Attribute> GetCustomAttributes(object onObject) =>
        onObject switch {
            Type t => t.GetCustomAttributes(),
            MemberInfo m => m.GetCustomAttributes(),
            ParameterInfo p => p.GetCustomAttributes(),
            _ => Array.Empty<Attribute>()
        };

    private static Nullability GetNullability(Attribute ncAttribute) => (Nullability)(ncAttribute?.GetType().GetField("Flag")?.GetValue(ncAttribute) ?? (byte)0);

    private static Nullability[] GetNullabilities(Attribute nAttribute) => (Nullability[])(nAttribute?.GetType().GetField("NullableFlags")?.GetValue(nAttribute) ?? Array.Empty<Nullability>());

    public static bool IsOptionalByNullability(MemberInfo member, IReflector reflector) => reflector.UseNullableReferenceTypesForOptionality && IsOptionalByNullability(member, member.DeclaringType);

    public static bool IsOptionalByNullability(ParameterInfo parameter, IReflector reflector) => reflector.UseNullableReferenceTypesForOptionality && IsOptionalByNullability(parameter, parameter.Member);

    private static bool IsOptionalByNullability(object member, object owner) {
        var ncAttribute = GetCustomAttributes(owner).SingleOrDefault(a => a.GetType().Name is "NullableContextAttribute");
        var nullableContext = GetNullability(ncAttribute);

        if (nullableContext is Nullability.Oblivious) {
            return false;
        }

        if (GetCustomAttributes(member).SingleOrDefault(a => a.GetType().Name is "NullableAttribute") is { } nAttribute) {
            return GetNullabilities(nAttribute).FirstOrDefault() is Nullability.Annotated;
        }

        return nullableContext is Nullability.Annotated;
    }

    private enum Nullability : byte {
        Oblivious = 0,
        NotAnnotated = 1,
        Annotated = 2
    }
}