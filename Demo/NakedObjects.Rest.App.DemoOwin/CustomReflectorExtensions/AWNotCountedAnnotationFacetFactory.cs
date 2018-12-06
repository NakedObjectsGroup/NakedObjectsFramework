using System.Collections.Immutable;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Reflect.FacetFactory;
using AdventureWorksModel;
using NakedObjects.Architecture.SpecImmutable;

namespace AWCustom
{
    public sealed class AWNotCountedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract
    {
        public AWNotCountedAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Collections) { }

        private static void Process(MemberInfo member, ISpecification holder)
        {
            var attribute = member.GetCustomAttribute<AWNotCountedAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification)
        {
            Process(property, specification);
        }

        private static INotCountedFacet Create(AWNotCountedAttribute attribute, ISpecification holder)
        {
            return attribute == null ? null : new NotCountedFacet(holder);
        }
    }

    public sealed class AWNotCountedAnnotationFacetFactoryParallel : NakedObjects.ParallelReflect.FacetFactory.AnnotationBasedFacetFactoryAbstract {
        public AWNotCountedAnnotationFacetFactoryParallel(int numericOrder)
            : base(numericOrder, FeatureType.Collections) { }

        private static void Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<AWNotCountedAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(property, specification);
            return metamodel;
        }

        private static INotCountedFacet Create(AWNotCountedAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new NotCountedFacet(holder);
        }
    }
}