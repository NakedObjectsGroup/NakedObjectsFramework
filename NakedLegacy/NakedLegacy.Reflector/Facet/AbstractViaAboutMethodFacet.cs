using System;
using System.Reflection;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Facet;

namespace NakedLegacy.Reflector.Facet;

public class AbstractViaAboutMethodFacet : FacetAbstract, IImperativeFacet {
    protected AbstractViaAboutMethodFacet(Type facetType, ISpecification holder, MethodInfo method, AboutHelpers.AboutType aboutType) : base(facetType, holder) {
        Method = method;
        AboutType = aboutType;
    }

    protected MethodInfo Method { get; }
    protected AboutHelpers.AboutType AboutType { get; }
    public MethodInfo GetMethod() => Method;

    protected IAbout InvokeAboutMethod(object target, AboutTypeCodes typeCode, params object[] proposedValues) {
        var about = AboutType.AboutFactory(typeCode);
        Method.Invoke(target, Method.GetParameters(about, proposedValues));
        return about;
    }

    public Func<object, object[], object> GetMethodDelegate() => throw new NotImplementedException();

    protected override string ToStringValues() => $"method={Method}";
}