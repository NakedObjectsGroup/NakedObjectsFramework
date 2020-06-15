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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    public sealed class Introspector : IIntrospector {
        private readonly IReflector reflector;
        private readonly ILogger<Introspector> logger;
        private MethodInfo[] methods;
        private List<IAssociationSpecImmutable> orderedFields;
        private List<IActionSpecImmutable> orderedObjectActions;
        private PropertyInfo[] properties;

        public Introspector(IReflector reflector, ILogger<Introspector> logger) {
            this.reflector = reflector;
            this.logger = logger;
        }

        private IClassStrategy ClassStrategy => reflector.ClassStrategy;

        private IFacetFactorySet FacetFactorySet => reflector.FacetFactorySet;

        private Type[] InterfacesTypes => IntrospectedType.GetInterfaces().Where(i => i.IsPublic).ToArray();

        private Type SuperclassType => IntrospectedType.BaseType;

        #region IIntrospector Members

        public Type IntrospectedType { get; private set; }

        public Type SpecificationType { get; private set; }

        /// <summary>
        ///     As per <see cref="MemberInfo.Name" />
        /// </summary>
        public string ClassName => IntrospectedType.Name;

        public IIdentifier Identifier { get; private set; }

        public string FullName => SpecificationType.GetProxiedTypeFullName();

        public string ShortName => TypeNameUtils.GetShortName(SpecificationType.Name);

        public IList<IAssociationSpecImmutable> Fields => orderedFields.ToImmutableList();

        public IList<IActionSpecImmutable> ObjectActions => orderedObjectActions.ToImmutableList();

        public ITypeSpecBuilder[] Interfaces { get; set; }
        public ITypeSpecBuilder Superclass { get; set; }

        public void IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(logger.LogAndReturn(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect)));
            }

            IntrospectedType = typeToIntrospect;
            SpecificationType = GetSpecificationType(typeToIntrospect);

            properties = typeToIntrospect.GetProperties();
            methods = GetNonPropertyMethods();
            Identifier = new IdentifierImpl(FullName);

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(methods);
            FacetFactorySet.Process(reflector, IntrospectedType, methodRemover, spec);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                Superclass = reflector.LoadSpecification(SuperclassType);
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (var interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    var interfaceSpec = reflector.LoadSpecification(interfaceType);
                    interfaceSpec.AddSubclass(spec);
                    interfaces.Add(interfaceSpec);
                }
            }

            Interfaces = interfaces.ToArray();
            IntrospectPropertiesAndCollections(spec);
            IntrospectActions(spec);
        }

        public IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) => throw new NotImplementedException();

        #endregion

        private static bool IsGenericEnumerableOrSet(Type type) =>
            CollectionUtils.IsGenericType(type, typeof(IEnumerable<>)) ||
            CollectionUtils.IsGenericType(type, typeof(ISet<>));

        private Type GetSpecificationType(Type type) {
            var actualType = ClassStrategy.GetType(type);

            if (IsGenericEnumerableOrSet(type)) {
                return type.GetGenericTypeDefinition();
            }

            if (type.IsArray && !(type.GetElementType().IsValueType || type.GetElementType() == typeof(string))) {
                return typeof(Array);
            }

            return actualType;
        }

        private void AddAsSubclass(ITypeSpecImmutable spec) => Superclass?.AddSubclass(spec);

        public void IntrospectPropertiesAndCollections(ITypeSpecImmutable spec) {
            var objectSpec = spec as IObjectSpecImmutable;
            orderedFields = objectSpec != null ? CreateSortedListOfMembers(FindAndCreateFieldSpecs(objectSpec)) : new List<IAssociationSpecImmutable>();
        }

        public void IntrospectActions(ITypeSpecImmutable spec) {
            // find the actions ...
            var findObjectActionMethods = FindActionMethods(spec);
            orderedObjectActions = CreateSortedListOfMembers(findObjectActionMethods);
        }

        private MethodInfo[] GetNonPropertyMethods() {
            // no better way to do this (ie no flag that indicates getter/setter)
            var allMethods = new List<MethodInfo>(IntrospectedType.GetMethods());
            foreach (var pInfo in properties) {
                allMethods.Remove(pInfo.GetGetMethod());
                allMethods.Remove(pInfo.GetSetMethod());
            }

            return allMethods.OrderBy(m => m, new SortActionsFirst(FacetFactorySet)).ToArray();
        }

        private IAssociationSpecImmutable[] FindAndCreateFieldSpecs(IObjectSpecImmutable spec) {
            // now create fieldSpecs for value properties, for collections and for reference properties        
            IList<PropertyInfo> collectionProperties = FacetFactorySet.FindCollectionProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToList();
            var collectionSpecs = CreateCollectionSpecs(collectionProperties, spec);

            // every other accessor is assumed to be a reference property.
            IList<PropertyInfo> allProperties = FacetFactorySet.FindProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToList();
            var refProperties = allProperties.Except(collectionProperties);
            var refSpecs = CreateRefPropertySpecs(refProperties, spec);

            return collectionSpecs.Union(refSpecs).ToArray();
        }

        private IEnumerable<IAssociationSpecImmutable> CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in collectionProperties) {
                IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                // create a collection property spec
                var returnType = property.PropertyType;
                var returnSpec = reflector.LoadSpecification<IObjectSpecImmutable>(returnType);
                var defaultType = typeof(object);
                var defaultSpec = reflector.LoadSpecification<IObjectSpecImmutable>(defaultType);

                var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections);
                specs.Add(collection);
            }

            return specs;
        }

        /// <summary>
        ///     Creates a list of Association fields for all the properties that use NakedObjects.
        /// </summary>
        private IEnumerable<IAssociationSpecImmutable> CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in foundProperties) {
                // create a reference property spec
                var identifier = new IdentifierImpl(FullName, property.Name);
                var propertyType = property.PropertyType;
                var propertySpec = reflector.LoadSpecification(propertyType);
                if (propertySpec is IServiceSpecImmutable) {
                    throw new ReflectionException(logger.LogAndReturn($"Type {propertyType.Name} is a service and cannot be used in public property {property.Name} on type {property.DeclaringType?.Name}. If the property is intended to be an injected service it should have a protected get."));
                }

                var referenceProperty = ImmutableSpecFactory.CreateOneToOneAssociationSpecImmutable(identifier, spec, propertySpec as IObjectSpecImmutable);

                // Process facets for the property
                FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), referenceProperty, FeatureType.Properties);
                specs.Add(referenceProperty);
            }

            return specs;
        }

        private IActionSpecImmutable[] FindActionMethods(ITypeSpecImmutable spec) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(methods.Where(m => m != null).ToArray(), reflector.ClassStrategy).Where(a => !FacetFactorySet.Filters(a, reflector.ClassStrategy)).ToArray();
            methods = methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // keep for loop as actions are nulled out within loop
            for (var i = 0; i < actions.Length; i++) {
                var actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    var fullMethodName = actionMethod.Name;

                    var parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                    // build action & its parameters   

                    if (actionMethod.ReturnType != typeof(void)) {
                        reflector.LoadSpecification(actionMethod.ReturnType);
                    }

                    IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());
                    var actionParams = parameterTypes.Select(pt => ImmutableSpecFactory.CreateActionParameterSpecImmutable(reflector.LoadSpecification<IObjectSpecImmutable>(pt), identifier)).ToArray();

                    var action = ImmutableSpecFactory.CreateActionSpecImmutable(identifier, spec, actionParams);

                    // Process facets on the action & parameters
                    FacetFactorySet.Process(reflector, actionMethod, new IntrospectorMethodRemover(actions), action, FeatureType.Actions);
                    for (var l = 0; l < actionParams.Length; l++) {
                        FacetFactorySet.ProcessParams(reflector, actionMethod, l, actionParams[l]);
                    }

                    actionSpecs.Add(action);
                }
            }

            return actionSpecs.ToArray();
        }

        private static List<T> CreateSortedListOfMembers<T>(T[] members) where T : IMemberSpecImmutable => members.OrderBy(m => m, new MemberOrderComparator<T>()).ToList();

        #region Nested type: IntrospectorMethodRemover

        #region Nested Type: DotnetIntrospectorMethodRemover

        private class IntrospectorMethodRemover : IMethodRemover {
            private readonly MethodInfo[] methods;

            public IntrospectorMethodRemover(MethodInfo[] methods) => this.methods = methods;

            #region IMethodRemover Members

            public void RemoveMethod(MethodInfo methodToRemove) {
                for (var i = 0; i < methods.Length; i++) {
                    if (methods[i] != null) {
                        if (methods[i].MemberInfoEquals(methodToRemove)) {
                            methods[i] = null;
                        }
                    }
                }
            }

            public void RemoveMethods(IList<MethodInfo> methodList) {
                for (var i = 0; i < methods.Length; i++) {
                    if (methods[i] != null) {
                        if (methodList.Any(methodToRemove => methods[i].MemberInfoEquals(methodToRemove))) {
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

        private class SortActionsFirst : IComparer<MethodInfo> {
            private readonly IFacetFactorySet factories;

            public SortActionsFirst(IFacetFactorySet factories) => this.factories = factories;

            #region IComparer<MethodInfo> Members

            public int Compare(MethodInfo x, MethodInfo y) {
                var xIsRecognised = x != null && factories.Recognizes(x);
                var yIsRecognised = y != null && factories.Recognizes(y);

                if (xIsRecognised == yIsRecognised) {
                    return 0;
                }

                return xIsRecognised ? 1 : -1;
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}