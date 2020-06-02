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
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.SpecImmutable;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect {
    public sealed class Introspector : IIntrospector {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Introspector));
        private readonly IReflector reflector;
        private MethodInfo[] methods;
        private IList<IAssociationSpecImmutable> orderedFields;
        private IList<IActionSpecImmutable> orderedObjectActions;
        private PropertyInfo[] properties;

        public Introspector(IReflector reflector) => this.reflector = reflector;

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

        public string FullName => SpecificationType.GetProxiedTypeFullName();

        public string ShortName => TypeNameUtils.GetShortName(SpecificationType.Name);

        public IIdentifier Identifier { get; private set; }

        public IList<IAssociationSpecImmutable> Fields => orderedFields.ToArray();

        public IList<IActionSpecImmutable> ObjectActions => orderedObjectActions.ToArray();

        public ITypeSpecBuilder[] Interfaces { get; set; }
        public ITypeSpecBuilder Superclass { get; set; }

        public void IntrospectType(Type typeToIntrospect, ITypeSpecImmutable specification) => throw new NotImplementedException();

        public IImmutableDictionary<string, ITypeSpecBuilder> IntrospectType(Type typeToIntrospect, ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!TypeUtils.IsPublic(typeToIntrospect)) {
                throw new ReflectionException(Log.LogAndReturn(string.Format(Resources.NakedObjects.DomainClassReflectionError, typeToIntrospect)));
            }

            IntrospectedType = typeToIntrospect;
            SpecificationType = GetSpecificationType(typeToIntrospect);

            properties = typeToIntrospect.GetProperties();
            methods = GetNonPropertyMethods();
            Identifier = new IdentifierImpl(FullName);

            // Process facets at object level
            // this will also remove some methods, such as the superclass methods.
            var methodRemover = new IntrospectorMethodRemover(methods);
            metamodel = FacetFactorySet.Process(reflector, IntrospectedType, methodRemover, spec, metamodel);

            if (SuperclassType != null && ClassStrategy.IsTypeToBeIntrospected(SuperclassType)) {
                (Superclass, metamodel) = reflector.LoadSpecification(SuperclassType, metamodel);
            }

            AddAsSubclass(spec);

            var interfaces = new List<ITypeSpecBuilder>();
            foreach (var interfaceType in InterfacesTypes) {
                if (interfaceType != null && ClassStrategy.IsTypeToBeIntrospected(interfaceType)) {
                    ITypeSpecBuilder interfaceSpec;
                    (interfaceSpec, metamodel) = reflector.LoadSpecification(interfaceType, metamodel);
                    interfaceSpec.AddSubclass(spec);
                    interfaces.Add(interfaceSpec);
                }
            }

            Interfaces = interfaces.ToArray();
            metamodel = IntrospectPropertiesAndCollections(spec, metamodel);
            metamodel = IntrospectActions(spec, metamodel);

            return metamodel;
        }

        #endregion

        private Type GetSpecificationType(Type type) =>
            FasterTypeUtils.IsGenericCollection(type)
                ? type.GetGenericTypeDefinition()
                : FasterTypeUtils.IsObjectArray(type)
                    ? typeof(Array)
                    : ClassStrategy.GetType(type);

        private void AddAsSubclass(ITypeSpecImmutable spec) => Superclass?.AddSubclass(spec);

        public IImmutableDictionary<string, ITypeSpecBuilder> IntrospectPropertiesAndCollections(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (spec is IObjectSpecImmutable objectSpec) {
                IAssociationSpecImmutable[] fields;
                (fields, metamodel) = FindAndCreateFieldSpecs(objectSpec, metamodel);
                orderedFields = CreateSortedListOfMembers(fields);
            }
            else {
                orderedFields = new List<IAssociationSpecImmutable>();
            }

            return metamodel;
        }

        public IImmutableDictionary<string, ITypeSpecBuilder> IntrospectActions(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // find the actions ...
            IActionSpecImmutable[] findObjectActionMethods;
            (findObjectActionMethods, metamodel) = FindActionMethods(spec, metamodel);
            orderedObjectActions = CreateSortedListOfMembers(findObjectActionMethods);
            return metamodel;
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

        private (IAssociationSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindAndCreateFieldSpecs(IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // now create fieldSpecs for value properties, for collections and for reference properties
            var collectionProperties = FacetFactorySet.FindCollectionProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            IEnumerable<IAssociationSpecImmutable> collectionSpecs;
            (collectionSpecs, metamodel) = CreateCollectionSpecs(collectionProperties, spec, metamodel);

            // every other accessor is assumed to be a reference property.
            var allProperties = FacetFactorySet.FindProperties(properties, ClassStrategy).Where(pi => !FacetFactorySet.Filters(pi, ClassStrategy)).ToArray();
            var refProperties = allProperties.Except(collectionProperties);

            IEnumerable<IAssociationSpecImmutable> refSpecs;
            (refSpecs, metamodel) = CreateRefPropertySpecs(refProperties, spec, metamodel);

            return (collectionSpecs.Union(refSpecs).ToArray(), metamodel);
        }

        private (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateCollectionSpecs(IEnumerable<PropertyInfo> collectionProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in collectionProperties) {
                IIdentifier identifier = new IdentifierImpl(FullName, property.Name);

                // create a collection property spec
                var returnType = property.PropertyType;
                IObjectSpecImmutable returnSpec;
                (returnSpec, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(returnType, metamodel);

                var defaultType = typeof(object);
                IObjectSpecImmutable defaultSpec;
                (defaultSpec, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(defaultType, metamodel);

                var collection = ImmutableSpecFactory.CreateOneToManyAssociationSpecImmutable(identifier, spec, returnSpec, defaultSpec);

                metamodel = FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), collection, FeatureType.Collections, metamodel);
                specs.Add(collection);
            }

            return (specs, metamodel);
        }

        /// <summary>
        ///     Creates a list of Association fields for all the properties that use NakedObjects.
        /// </summary>
        private (IEnumerable<IAssociationSpecImmutable>, IImmutableDictionary<string, ITypeSpecBuilder>) CreateRefPropertySpecs(IEnumerable<PropertyInfo> foundProperties, IObjectSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var specs = new List<IAssociationSpecImmutable>();

            foreach (var property in foundProperties) {
                // create a reference property spec
                var identifier = new IdentifierImpl(FullName, property.Name);
                var propertyType = property.PropertyType;
                IObjectSpecImmutable propertySpec;
                (propertySpec, metamodel) = reflector.LoadSpecification<IObjectSpecImmutable>(propertyType, metamodel);

                if (propertySpec == null) {
                    throw new ReflectionException(Log.LogAndReturn($"Type {propertyType.Name} is a service and cannot be used in public property {property.Name} on type {property.DeclaringType?.Name}. If the property is intended to be an injected service it should have a protected get."));
                }

                var referenceProperty = ImmutableSpecFactory.CreateOneToOneAssociationSpecImmutable(identifier, spec, propertySpec);

                // Process facets for the property
                metamodel = FacetFactorySet.Process(reflector, property, new IntrospectorMethodRemover(methods), referenceProperty, FeatureType.Properties, metamodel);
                specs.Add(referenceProperty);
            }

            return (specs, metamodel);
        }

        private (IActionSpecImmutable[], IImmutableDictionary<string, ITypeSpecBuilder>) FindActionMethods(ITypeSpecImmutable spec, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actionSpecs = new List<IActionSpecImmutable>();
            var actions = FacetFactorySet.FindActions(methods.Where(m => m != null).ToArray(), reflector.ClassStrategy).Where(a => !FacetFactorySet.Filters(a, reflector.ClassStrategy)).ToArray();
            methods = methods.Except(actions).ToArray();

            // ReSharper disable once ForCanBeConvertedToForeach
            // keep a "for loop" as actions are nulled out within loop
            for (var i = 0; i < actions.Length; i++) {
                var actionMethod = actions[i];

                // actions are potentially being nulled within this loop
                if (actionMethod != null) {
                    var fullMethodName = actionMethod.Name;

                    var parameterTypes = actionMethod.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();

                    // build action & its parameters

                    if (actionMethod.ReturnType != typeof(void)) {
                        (_, metamodel) = reflector.LoadSpecification(actionMethod.ReturnType, metamodel);
                    }

                    IIdentifier identifier = new IdentifierImpl(FullName, fullMethodName, actionMethod.GetParameters().ToArray());

                    var actionParams = new List<IActionParameterSpecImmutable>();

                    foreach (var pt in parameterTypes) {
                        IObjectSpecBuilder oSpec;
                        (oSpec, metamodel) = reflector.LoadSpecification<IObjectSpecBuilder>(pt, metamodel);
                        var actionSpec = ImmutableSpecFactory.CreateActionParameterSpecImmutable(oSpec, identifier);
                        actionParams.Add(actionSpec);
                    }

                    var action = ImmutableSpecFactory.CreateActionSpecImmutable(identifier, spec, actionParams.ToArray());

                    // Process facets on the action & parameters
                    metamodel = FacetFactorySet.Process(reflector, actionMethod, new IntrospectorMethodRemover(actions), action, FeatureType.Actions, metamodel);
                    for (var l = 0; l < actionParams.Count; l++) {
                        metamodel = FacetFactorySet.ProcessParams(reflector, actionMethod, l, actionParams[l], metamodel);
                    }

                    actionSpecs.Add(action);
                }
            }

            return (actionSpecs.ToArray(), metamodel);
        }

        private static IList<T> CreateSortedListOfMembers<T>(T[] members) where T : IMemberSpecImmutable => members.OrderBy(m => m, new MemberOrderComparator<T>()).ToArray();

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