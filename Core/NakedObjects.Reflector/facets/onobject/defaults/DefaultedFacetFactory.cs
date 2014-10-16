// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class DefaultedFacetFactory : AnnotationBasedFacetFactoryAbstract, INakedObjectConfigurationAware {
        public DefaultedFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ObjectsPropertiesAndParameters) { }

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            return FacetUtils.AddFacet(Create(type, specification));
        }

        private static IDefaultedFacet Create(Type type, ISpecification holder) {
            var annotation = type.GetCustomAttributeByReflection<DefaultedAttribute>();

            // create from annotation, if present
            if (annotation != null) {
                var facet = TypeUtils.CreateGenericInstance<IDefaultedFacet>(typeof (DefaultedFacetAnnotation<>),
                                                                             new[] {type},
                                                                             new object[] {type, holder});


                if (facet.IsValid) {
                    return facet;
                }
            }


            return null;
        }

        /// <summary>
        ///     If there is a <see cref="IDefaultedFacet" />on the properties return Type, then installs a
        ///     <see cref="IPropertyDefaultFacet" /> for the property with the same default.
        /// </summary>
        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            // don't overwrite any defaults already picked up
            if (specification.ContainsFacet(typeof (IPropertyDefaultFacet))) {
                return false;
            }

            // try to infer defaults from the underlying return Type
            Type returnType = method.ReturnType;
            IDefaultedFacet returnTypeDefaultedFacet = GetDefaultedFacet(returnType);
            if (returnTypeDefaultedFacet != null) {
                var propertyFacet = new PropertyDefaultFacetDerivedFromDefaultedFacet(returnTypeDefaultedFacet, specification);
                return FacetUtils.AddFacet(propertyFacet);
            }
            return false;
        }

        /// <summary>
        ///     If there is a <see cref="IDefaultedFacet" /> on any of the action's parameter types, then installs a
        ///     <see cref="IActionDefaultsFacet" /> for the action.
        /// </summary>
        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            // don't overwrite any defaults already picked up
            if (holder.ContainsFacet(typeof (IActionDefaultsFacet))) {
                return false;
            }

            // try to infer defaults from any of the parameter's underlying types

            Type paramType = method.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray()[paramNum];
            IDefaultedFacet defaultedFacet = GetDefaultedFacet(paramType);
            if (defaultedFacet != null) {
                return FacetUtils.AddFacet(new ActionDefaultsFacetDerivedFromDefaultedFacets(defaultedFacet, holder));
            }
            return false;
        }

        private IDefaultedFacet GetDefaultedFacet(Type paramType) {
            var paramTypeSpec = Reflector.LoadSpecification(paramType);
            return paramTypeSpec.GetFacet<IDefaultedFacet>();
        }
    }
}