// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.


using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect {
    public abstract class Introspector : IIntrospector {
        protected IList<IAssociationSpecImmutable> OrderedFields { get; set; }
        protected IList<IActionSpecImmutable> OrderedObjectActions { get; set; }
        protected PropertyInfo[] Properties { get; set; }
        protected IReflector Reflector { get; set; }
        protected MethodInfo[] Methods { get; set; }


        protected IClassStrategy ClassStrategy { get; init;  }

        protected IFacetFactorySet FacetFactorySet { get; init; }

        protected Type[] InterfacesTypes => IntrospectedType.GetInterfaces().Where(i => i.IsPublic).ToArray();

        protected Type SuperclassType => IntrospectedType.BaseType;

        protected void AddAsSubclass(ITypeSpecImmutable spec) => Superclass?.AddSubclass(spec);

        private static IList<T> CreateSortedListOfMembers<T>(T[] members) where T : IMemberSpecImmutable => members.OrderBy(m => m, new MemberOrderComparator<T>()).ToArray();

        protected abstract (IActionSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindActionMethods(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);


        protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPropertiesAndCollections(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel)
        {
            if (spec is IObjectSpecImmutable objectSpec)
            {
                IAssociationSpecImmutable[] fields;
                (fields, metamodel) = FindAndCreateFieldSpecs(objectSpec, metamodel);
                OrderedFields = CreateSortedListOfMembers(fields);
            }
            else
            {
                OrderedFields = new List<IAssociationSpecImmutable>();
            }

            return metamodel;
        }

        protected IImmutableDictionary<string, ITypeSpecBuilder> IntrospectActions(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel)
        {
            // find the actions ...
            IActionSpecImmutable[] findObjectActionMethods;
            (findObjectActionMethods, metamodel) = FindActionMethods(spec, metamodel);
            OrderedObjectActions = CreateSortedListOfMembers(findObjectActionMethods);
            return metamodel;
        }


        protected MethodInfo[] GetNonPropertyMethods()
        {
            // no better way to do this (ie no flag that indicates getter/setter)
            var allMethods = new List<MethodInfo>(IntrospectedType.GetMethods());
            foreach (var pInfo in Properties)
            {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }

            return allMethods.OrderBy(m => m, new FunctionalIntrospector.SortActionsFirst(FacetFactorySet)).ToArray();
        }

        protected (IAssociationSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindAndCreateFieldSpecs(IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel)
        {
            // now create fieldSpecs for value properties, for collections and for reference properties
            var collectionProperties = FacetFactorySet.FindCollectionProperties(Properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            IEnumerable<IAssociationSpecImmutable> collectionSpecs;
            (collectionSpecs, metamodel) = CreateCollectionSpecs(collectionProperties, spec, metamodel);

            // every other accessor is assumed to be a reference property.
            var allProperties = FacetFactorySet.FindProperties(Properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            var refProperties = allProperties.Except(collectionProperties);

            IEnumerable<IAssociationSpecImmutable> refSpecs;
            (refSpecs, metamodel) = CreateRefPropertySpecs(refProperties, spec, metamodel);

            return (collectionSpecs.Union(refSpecs).ToArray(), metamodel);
        }

        protected (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel)
        {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in collectionProperties)
            {
                IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                // create a collection property spec
                var returnType = property.PropertyType;
                IObjectSpecImmutable returnSpec;
                (returnSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(returnType, ClassStrategy, metamodel);

                var defaultType = typeof(object);
                IObjectSpecImmutable defaultSpec;
                (defaultSpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(defaultType, ClassStrategy, metamodel);

                var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                metamodel = FacetFactorySet.Process(Reflector, ClassStrategy, property, new FunctionalIntrospector.IntrospectorMethodRemover(Methods), collection, FeatureType.Collections, metamodel);
                specs.Add(collection);
            }

            return (specs, metamodel);
        }

        protected (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel)
        {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in foundProperties)
            {
                // create a reference property spec
                var identifier = new IdentifierImpl(FullName, property.Name);
                var propertyType = property.PropertyType;
                IObjectSpecImmutable propertySpec;
                (propertySpec, metamodel) = Reflector.LoadSpecification<IObjectSpecImmutable>(propertyType, ClassStrategy, metamodel);

                if (propertySpec == null)
                {
                    throw new ReflectionException($"Type {propertyType.Name} is a service and cannot be used in public property {property.Name} on type {property.DeclaringType?.Name}. If the property is intended to be an injected service it should have a protected get.");
                }

                var referenceProperty = ImmutableSpecFactory.CreateOneToOneAssociationSpecImmutable(identifier, spec, propertySpec);

                // Process facets for the property
                metamodel = FacetFactorySet.Process(Reflector, ClassStrategy, property, new FunctionalIntrospector.IntrospectorMethodRemover(Methods), referenceProperty, FeatureType.Properties, metamodel);
                specs.Add(referenceProperty);
            }

            return (specs, metamodel);
        }



        #region Nested type: IntrospectorMethodRemover

        #region Nested Type: DotnetIntrospectorMethodRemover

        protected class IntrospectorMethodRemover : IMethodRemover
        {
            private readonly MethodInfo[] methods;

            public IntrospectorMethodRemover(MethodInfo[] methods) => this.methods = methods;

            #region IMethodRemover Members

            public void RemoveMethod(MethodInfo methodToRemove)
            {
                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i] != null)
                    {
                        if (methods[i].MemberInfoEquals(methodToRemove))
                        {
                            methods[i] = null;
                        }
                    }
                }
            }

            public void RemoveMethods(IList<MethodInfo> methodList)
            {
                for (var i = 0; i < methods.Length; i++)
                {
                    if (methods[i] != null)
                    {
                        if (methodList.Any(methodToRemove => methods[i].MemberInfoEquals(methodToRemove)))
                        {
                            methods[i] = null;
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #endregion

        #region Nested type: SortActionsFirst

        protected class SortActionsFirst : IComparer<MethodInfo>
        {
            private readonly IFacetFactorySet factories;

            public SortActionsFirst(IFacetFactorySet factories) => this.factories = factories;

            #region IComparer<MethodInfo> Members

            public int Compare(MethodInfo x, MethodInfo y)
            {
                var xIsRecognised = x != null && factories.Recognizes(x);
                var yIsRecognised = y != null && factories.Recognizes(y);

                if (xIsRecognised == yIsRecognised)
                {
                    return 0;
                }

                return xIsRecognised ? 1 : -1;
            }

            #endregion
        }

        #endregion





        public Type IntrospectedType { get; protected set; }

        public Type SpecificationType { get; protected set; }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        public string ClassName => IntrospectedType.Name;

        public string FullName => SpecificationType.GetProxiedTypeFullName();

        public string ShortName => TypeNameUtils.GetShortName(SpecificationType.Name);

        public IIdentifier Identifier { get; protected set; }

        public IList<IAssociationSpecImmutable> Fields => OrderedFields.ToArray();

        public IList<IActionSpecImmutable> ObjectActions => OrderedObjectActions.ToArray();

        public ITypeSpecBuilder[] Interfaces { get; set; }
        public ITypeSpecBuilder Superclass { get; set; }

        public abstract IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel);
    }
}